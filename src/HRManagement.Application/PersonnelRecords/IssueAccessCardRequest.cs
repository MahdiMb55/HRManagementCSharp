namespace HRManagement.Application.PersonnelRecords;

public sealed record IssueAccessCardRequest(
    long EmployeeId,
    string? CardNumber,
    DateOnly StartDate,
    string? Notes);
