using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class IssuedLetter : Entity
{
    private IssuedLetter()
    {
    }

    private IssuedLetter(
        long employeeId,
        long letterTemplateId,
        string letterNumber,
        DateOnly issueDate,
        string? subject,
        long outputFileId,
        DateTime createdAtUtc)
    {
        EmployeeId = employeeId;
        LetterTemplateId = letterTemplateId;
        LetterNumber = letterNumber.Trim();
        IssueDate = issueDate;
        Subject = string.IsNullOrWhiteSpace(subject) ? null : subject.Trim();
        OutputFileId = outputFileId;
        CreatedAtUtc = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime();
    }

    public long EmployeeId { get; private set; }
    public long LetterTemplateId { get; private set; }
    public string LetterNumber { get; private set; } = string.Empty;
    public DateOnly IssueDate { get; private set; }
    public string? Subject { get; private set; }
    public long OutputFileId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<IssuedLetter> Create(
        long employeeId,
        long letterTemplateId,
        string? letterNumber,
        DateOnly issueDate,
        string? subject,
        long outputFileId,
        DateTime createdAtUtc)
    {
        if (employeeId <= 0 || letterTemplateId <= 0 || outputFileId <= 0 || string.IsNullOrWhiteSpace(letterNumber))
        {
            return ValidationResult<IssuedLetter>.Failure(
                "issued_letter.required",
                "کارمند، قالب، شماره نامه و فایل خروجی الزامی است.");
        }

        return ValidationResult<IssuedLetter>.Success(
            new IssuedLetter(employeeId, letterTemplateId, letterNumber, issueDate, subject, outputFileId, createdAtUtc));
    }
}
