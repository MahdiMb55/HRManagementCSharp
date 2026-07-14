using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class EmployeeDocument : AuditableEntity
{
    private EmployeeDocument()
    {
    }

    private EmployeeDocument(
        long employeeId,
        long categoryId,
        string title,
        string? description,
        DateTime nowUtc)
    {
        EmployeeId = employeeId;
        CategoryId = categoryId;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        InitializeTimestamps(nowUtc);
    }

    public long EmployeeId { get; private set; }
    public long CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static ValidationResult<EmployeeDocument> Create(
        long employeeId,
        long categoryId,
        string? title,
        string? description,
        DateTime nowUtc)
    {
        if (employeeId <= 0 || categoryId <= 0)
        {
            return ValidationResult<EmployeeDocument>.Failure(
                "employee_document.required",
                "کارمند و دسته سند الزامی است.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return ValidationResult<EmployeeDocument>.Failure(
                "employee_document.title.required",
                "عنوان سند الزامی است.");
        }

        return ValidationResult<EmployeeDocument>.Success(
            new EmployeeDocument(employeeId, categoryId, title, description, nowUtc));
    }
}
