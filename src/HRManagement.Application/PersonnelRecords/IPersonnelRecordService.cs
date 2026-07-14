namespace HRManagement.Application.PersonnelRecords;

public interface IPersonnelRecordService
{
    Task<PersonnelRecordResult> AddEducationRecordAsync(
        AddEducationRecordRequest request,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> AddDependentAsync(
        AddDependentRequest request,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> AddBankAccountAsync(
        AddBankAccountRequest request,
        CancellationToken cancellationToken);

    Task<PersonnelRecordResult> IssueAccessCardAsync(
        IssueAccessCardRequest request,
        CancellationToken cancellationToken);
}
