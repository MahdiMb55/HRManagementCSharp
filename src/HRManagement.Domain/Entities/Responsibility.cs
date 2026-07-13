using HRManagement.Domain.Common;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Entities;

public sealed class Responsibility : AuditableEntity
{
    private Responsibility()
    {
    }

    private Responsibility(string title, DateTime nowUtc)
    {
        Title = title.Trim();
        NormalizedTitle = PersianTextNormalizer.Normalize(Title);
        IsActive = true;
        InitializeTimestamps(nowUtc);
    }

    public string Title { get; private set; } = string.Empty;

    public string NormalizedTitle { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public static ValidationResult<Responsibility> Create(string? title, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return ValidationResult<Responsibility>.Failure(
                "responsibility.title.required",
                "عنوان مسئولیت الزامی است.");
        }

        return ValidationResult<Responsibility>.Success(new Responsibility(title, nowUtc));
    }
}
