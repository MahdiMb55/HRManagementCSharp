namespace HRManagement.Application.Letters;

public sealed record IssueLetterRequest(
    long TemplateId,
    IReadOnlyCollection<long> EmployeeIds,
    string LetterNumber,
    DateOnly IssueDate,
    string? Subject);
