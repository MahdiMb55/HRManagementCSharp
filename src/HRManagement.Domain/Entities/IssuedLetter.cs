using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class IssuedLetter : Entity
{
    public long EmployeeId { get; private set; }
    public long LetterTemplateId { get; private set; }
    public string LetterNumber { get; private set; } = string.Empty;
    public DateOnly IssueDate { get; private set; }
    public string? Subject { get; private set; }
    public long OutputFileId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
