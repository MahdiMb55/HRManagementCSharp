using HRManagement.Domain.Entities;
using HRManagement.Domain.Identity;

namespace HRManagement.Application.PersonnelRecords;

public interface IPersonnelRecordRepository
{
    Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken);

    Task<PersonnelRecordResult> AddEducationRecordAsync(
        EducationRecord record,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> AddDependentAsync(
        long employeeId,
        NationalCode nationalCode,
        Func<long, DateTime, Person> createPerson,
        Func<long, DateTime, EmployeeDependent> createDependent,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> AddBankAccountAsync(
        EmployeeBankAccount account,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> IssueAccessCardAsync(
        long employeeId,
        string cardNumber,
        DateOnly startDate,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
