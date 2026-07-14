using HRManagement.Application.Letters;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Letters;

public sealed class EfLetterRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : ILetterRepository
{
    public async Task<IReadOnlyList<LetterTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.LetterTemplates
            .AsNoTracking()
            .Where(template => template.IsActive)
            .OrderBy(template => template.Title)
            .Select(template => new LetterTemplateDto(template.Id, template.Title, template.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<LetterTemplateSource?> GetTemplateSourceAsync(long templateId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await (
            from template in context.LetterTemplates.AsNoTracking()
            join file in context.ManagedFiles.AsNoTracking() on template.ManagedFileId equals file.Id
            where template.Id == templateId && template.IsActive && !file.IsInTrash
            select new LetterTemplateSource(template.Id, template.Title, file.RelativePath))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeLetterProjection>> GetEmployeeProjectionsAsync(
        IReadOnlyCollection<long> employeeIds,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var ids = employeeIds.ToArray();
        var companyName = await context.CompanyProfiles
            .AsNoTracking()
            .Where(profile => profile.Id == 1)
            .Select(profile => profile.CompanyName)
            .SingleOrDefaultAsync(cancellationToken);
        return await (
            from employee in context.Employees.AsNoTracking()
            join person in context.Persons.AsNoTracking() on employee.PersonId equals person.Id
            where ids.Contains(employee.Id) && !employee.IsDeleted
            select new EmployeeLetterProjection(
                employee.Id,
                person.FirstName,
                person.LastName,
                employee.PersonnelNumber,
                person.NationalCode,
                employee.MobileNumber,
                (
                    from history in context.EmployeeDepartmentHistories
                    join department in context.Departments on history.DepartmentId equals department.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null
                    select department.Name).FirstOrDefault(),
                (
                    from history in context.EmployeeResponsibilityHistories
                    join responsibility in context.Responsibilities on history.ResponsibilityId equals responsibility.Id
                    where history.EmployeeId == employee.Id && history.EndDate == null && history.IsPrimary
                    select responsibility.Title).FirstOrDefault(),
                companyName))
            .ToListAsync(cancellationToken);
    }

    public async Task<long> RegisterTemplateAsync(
        string title,
        string? description,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        context.ManagedFiles.Add(file);
        await context.SaveChangesAsync(cancellationToken);
        var saved = LetterTemplate.Create(title, description, file.Id, nowUtc).Value!;
        context.LetterTemplates.Add(saved);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return saved.Id;
    }

    public async Task SaveIssuedLetterAsync(
        long employeeId,
        long templateId,
        string letterNumber,
        DateOnly issueDate,
        string? subject,
        ManagedFile outputFile,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        context.ManagedFiles.Add(outputFile);
        await context.SaveChangesAsync(cancellationToken);
        var saved = IssuedLetter.Create(
            employeeId,
            templateId,
            letterNumber,
            issueDate,
            subject,
            outputFile.Id,
            nowUtc).Value!;
        context.IssuedLetters.Add(saved);
        context.AuditLogs.Add(AuditLog.Create(nameof(IssuedLetter), saved.Id, "letter.issued", "Letter issued.", saved.CreatedAtUtc));
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
