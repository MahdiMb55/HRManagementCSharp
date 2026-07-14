using HRManagement.Application.PersonnelRecords;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Identity;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.PersonnelRecords;

public sealed class EfPersonnelRecordRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IPersonnelRecordRepository
{
    public async Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees.AnyAsync(
            employee => employee.Id == employeeId && !employee.IsDeleted,
            cancellationToken);
    }

    public async Task<PersonnelRecordResult> AddEducationRecordAsync(
        EducationRecord record,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        if (record.IsPrimary)
        {
            var currentPrimaryRecords = await context.EducationRecords
                .Where(existing => existing.EmployeeId == record.EmployeeId && existing.IsPrimary && !existing.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var currentPrimaryRecord in currentPrimaryRecords)
            {
                currentPrimaryRecord.ClearPrimary(nowUtc);
            }
        }

        context.EducationRecords.Add(record);
        await context.SaveChangesAsync(cancellationToken);
        AddAudit(context, nameof(EducationRecord), record.Id, "education.added", "Education record added.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PersonnelRecordResult.Success(record.Id);
    }

    public async Task<PersonnelRecordResult> AddDependentAsync(
        long employeeId,
        NationalCode nationalCode,
        Func<long, DateTime, Person> createPerson,
        Func<long, DateTime, EmployeeDependent> createDependent,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var employeePersonId = await context.Employees
            .Where(employee => employee.Id == employeeId && !employee.IsDeleted)
            .Select(employee => (long?)employee.PersonId)
            .SingleOrDefaultAsync(cancellationToken);
        if (employeePersonId is null)
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmployeeNotFound,
                "کارمند موردنظر پیدا نشد.");
        }

        var dependentPerson = await context.Persons
            .SingleOrDefaultAsync(person => person.NationalCode == nationalCode.Value, cancellationToken);
        if (dependentPerson is null)
        {
            dependentPerson = createPerson(0, nowUtc);
            context.Persons.Add(dependentPerson);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (dependentPerson.Id == employeePersonId.Value)
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.SelfDependent,
                "کارمند نمی‌تواند فرد تحت تکفل خودش باشد.");
        }

        var alreadyExists = await context.EmployeeDependents.AnyAsync(
            dependent =>
                dependent.EmployeeId == employeeId &&
                dependent.DependentPersonId == dependentPerson.Id &&
                !dependent.IsDeleted,
            cancellationToken);
        if (alreadyExists)
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.DuplicateRecord,
                "این فرد قبلاً برای این کارمند ثبت شده است.");
        }

        var employeeDependent = createDependent(dependentPerson.Id, nowUtc);
        context.EmployeeDependents.Add(employeeDependent);
        await context.SaveChangesAsync(cancellationToken);
        AddAudit(context, nameof(EmployeeDependent), employeeDependent.Id, "dependent.added", "Dependent added.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PersonnelRecordResult.Success(employeeDependent.Id);
    }

    public async Task<PersonnelRecordResult> AddBankAccountAsync(
        EmployeeBankAccount account,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        if (account.IsPrimary)
        {
            var currentPrimaryAccounts = await context.EmployeeBankAccounts
                .Where(existing => existing.EmployeeId == account.EmployeeId && existing.IsPrimary && existing.IsActive && !existing.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var currentPrimaryAccount in currentPrimaryAccounts)
            {
                currentPrimaryAccount.ClearPrimary(nowUtc);
            }
        }

        if (!string.IsNullOrWhiteSpace(account.CardNumber) &&
            await context.EmployeeBankAccounts.AnyAsync(
                existing => existing.CardNumber == account.CardNumber && !existing.IsDeleted,
                cancellationToken))
        {
            return Duplicate("شماره کارت بانکی قبلاً ثبت شده است.");
        }

        if (!string.IsNullOrWhiteSpace(account.Iban) &&
            await context.EmployeeBankAccounts.AnyAsync(
                existing => existing.Iban == account.Iban && !existing.IsDeleted,
                cancellationToken))
        {
            return Duplicate("شماره شبا قبلاً ثبت شده است.");
        }

        context.EmployeeBankAccounts.Add(account);
        await context.SaveChangesAsync(cancellationToken);
        AddAudit(context, nameof(EmployeeBankAccount), account.Id, "bank_account.added", "Bank account added.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PersonnelRecordResult.Success(account.Id);
    }

    public async Task<PersonnelRecordResult> IssueAccessCardAsync(
        long employeeId,
        string cardNumber,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var employmentPeriodId = await context.EmploymentPeriods
            .Where(period => period.EmployeeId == employeeId && period.EndDate == null)
            .Select(period => (long?)period.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (employmentPeriodId is null)
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.EmploymentPeriodRequired,
                "برای ثبت کارت تردد، دوره استخدام فعال لازم است.");
        }

        var activeCard = await context.EmployeeAccessCards
            .SingleOrDefaultAsync(card => card.EmployeeId == employeeId && card.EndDate == null, cancellationToken);
        if (activeCard is not null)
        {
            var endResult = activeCard.End(startDate);
            if (!endResult.IsSuccess)
            {
                return PersonnelRecordResult.Failure(
                    PersonnelRecordFailureCode.InvalidInput,
                    endResult.Errors[0].Message);
            }
        }

        if (await context.EmployeeAccessCards.AnyAsync(
            card => card.CardNumber == cardNumber && card.EndDate == null,
            cancellationToken))
        {
            return Duplicate("شماره کارت تردد فعال قبلاً ثبت شده است.");
        }

        var newCard = EmployeeAccessCard.Create(
            employeeId,
            employmentPeriodId.Value,
            cardNumber,
            startDate,
            notes,
            nowUtc);
        if (!newCard.IsSuccess)
        {
            return PersonnelRecordResult.Failure(
                PersonnelRecordFailureCode.InvalidInput,
                newCard.Errors[0].Message);
        }

        context.EmployeeAccessCards.Add(newCard.Value!);
        await context.SaveChangesAsync(cancellationToken);
        AddAudit(context, nameof(EmployeeAccessCard), newCard.Value!.Id, "access_card.issued", "Access card issued.", nowUtc);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PersonnelRecordResult.Success(newCard.Value!.Id);
    }

    private static PersonnelRecordResult Duplicate(string message) =>
        PersonnelRecordResult.Failure(PersonnelRecordFailureCode.DuplicateRecord, message);

    private static void AddAudit(
        HrManagementDbContext context,
        string entityType,
        long entityId,
        string action,
        string description,
        DateTime nowUtc) =>
        context.AuditLogs.Add(AuditLog.Create(entityType, entityId, action, description, nowUtc));
}
