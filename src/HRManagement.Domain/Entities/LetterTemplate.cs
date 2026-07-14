using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class LetterTemplate : AuditableEntity
{
    private LetterTemplate()
    {
    }

    private LetterTemplate(
        string title,
        string? description,
        long managedFileId,
        DateTime nowUtc)
    {
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ManagedFileId = managedFileId;
        IsActive = true;
        InitializeTimestamps(nowUtc);
    }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long ManagedFileId { get; private set; }
    public bool IsActive { get; private set; }

    public static ValidationResult<LetterTemplate> Create(
        string? title,
        string? description,
        long managedFileId,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(title) || managedFileId <= 0)
        {
            return ValidationResult<LetterTemplate>.Failure(
                "letter_template.required",
                "عنوان قالب و فایل قالب الزامی است.");
        }

        return ValidationResult<LetterTemplate>.Success(new LetterTemplate(title, description, managedFileId, nowUtc));
    }
}
