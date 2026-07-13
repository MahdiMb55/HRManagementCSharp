using HRManagement.Application.Employees;
using HRManagement.Domain.Entities;
using HRManagement.Domain.Identity;
using HRManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Infrastructure.Employees;

public sealed class EfEmployeeWriteRepository(
    IDbContextFactory<HrManagementDbContext> contextFactory) : IEmployeeWriteRepository
{
    public async Task<EmployeeEditDto?> FindForEditAsync(
        long employeeId,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await (
            from employee in context.Employees.AsNoTracking()
            join person in context.Persons.AsNoTracking() on employee.PersonId equals person.Id
            where employee.Id == employeeId && !employee.IsDeleted
            select new EmployeeEditDto(
                employee.Id,
                person.FirstName,
                person.LastName,
                employee.PersonnelNumber,
                person.NationalCode,
                employee.FatherName,
                person.Gender,
                person.BirthDate,
                employee.MobileNumber))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> NationalCodeExistsAsync(
        string nationalCode,
        long? excludingEmployeeId,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        long? excludedPersonId = null;
        if (excludingEmployeeId is long employeeId)
        {
            excludedPersonId = await context.Employees
                .Where(employee => employee.Id == employeeId)
                .Select(employee => (long?)employee.PersonId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return await context.Persons.AnyAsync(
            person => person.NationalCode == nationalCode && person.Id != excludedPersonId,
            cancellationToken);
    }

    public async Task<bool> PersonnelNumberExistsAsync(
        string personnelNumber,
        long? excludingEmployeeId,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.Employees.AnyAsync(
            employee => employee.PersonnelNumber == personnelNumber && employee.Id != excludingEmployeeId,
            cancellationToken);
    }

    public async Task<long> SaveAsync(
        ValidatedEmployeeSave employeeSave,
        CancellationToken cancellationToken)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var nationalCode = NationalCode.Create(employeeSave.NationalCode).Value
            ?? throw new InvalidOperationException("Validated national code could not be reconstructed.");
        var personnelNumber = PersonnelNumber.Create(employeeSave.PersonnelNumber).Value
            ?? throw new InvalidOperationException("Validated personnel number could not be reconstructed.");
        var today = DateOnly.FromDateTime(employeeSave.OccurredAtUtc);

        Employee employee;
        if (employeeSave.EmployeeId is null)
        {
            var person = Person.Create(
                employeeSave.FirstName,
                employeeSave.LastName,
                nationalCode,
                employeeSave.Gender,
                employeeSave.BirthDate,
                today,
                employeeSave.OccurredAtUtc).Value
                ?? throw new InvalidOperationException("Validated person could not be created.");
            context.Persons.Add(person);
            await context.SaveChangesAsync(cancellationToken);

            employee = Employee.Create(person.Id, personnelNumber, employeeSave.OccurredAtUtc).Value
                ?? throw new InvalidOperationException("Validated employee could not be created.");
            employee.UpdateBasicDetails(employeeSave.FatherName, employeeSave.MobileNumber, employeeSave.OccurredAtUtc);
            context.Employees.Add(employee);
            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            employee = await context.Employees.SingleOrDefaultAsync(
                candidate => candidate.Id == employeeSave.EmployeeId.Value,
                cancellationToken)
                ?? throw new InvalidOperationException("Employee disappeared during save.");
            var person = await context.Persons.SingleAsync(
                candidate => candidate.Id == employee.PersonId,
                cancellationToken);
            var updateResult = person.UpdateBasicIdentity(
                employeeSave.FirstName,
                employeeSave.LastName,
                nationalCode,
                employeeSave.Gender,
                employeeSave.BirthDate,
                today,
                employeeSave.OccurredAtUtc);
            if (!updateResult.IsSuccess)
            {
                throw new InvalidOperationException("Validated person update failed.");
            }

            employee.UpdateBasicDetails(employeeSave.FatherName, employeeSave.MobileNumber, employeeSave.OccurredAtUtc);
        }

        context.AuditLogs.Add(AuditLog.Create(
            nameof(Employee),
            employee.Id,
            employeeSave.AuditAction,
            employeeSave.EmployeeId is null ? "پرونده کارمند ایجاد شد." : "اطلاعات پایه کارمند ویرایش شد.",
            employeeSave.OccurredAtUtc));
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return employee.Id;
    }
}
