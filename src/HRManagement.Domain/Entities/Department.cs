using HRManagement.Domain.Common;
using HRManagement.Domain.Text;

namespace HRManagement.Domain.Entities;

public sealed class Department : AuditableEntity
{
    private Department()
    {
    }

    private Department(string name, long? parentDepartmentId, DateTime nowUtc)
    {
        Name = name.Trim();
        NormalizedName = PersianTextNormalizer.Normalize(Name);
        ParentDepartmentId = parentDepartmentId;
        IsActive = true;
        InitializeTimestamps(nowUtc);
    }

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public long? ParentDepartmentId { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public static ValidationResult<Department> Create(
        string? name,
        long? parentDepartmentId,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationResult<Department>.Failure(
                "department.name.required",
                "نام واحد سازمانی الزامی است.");
        }

        return ValidationResult<Department>.Success(
            new Department(name, parentDepartmentId, nowUtc));
    }

    public static bool WouldCreateCycle(
        long departmentId,
        long candidateParentId,
        IReadOnlyDictionary<long, long?> parents)
    {
        if (departmentId == candidateParentId)
        {
            return true;
        }

        var visited = new HashSet<long>();
        long? current = candidateParentId;
        while (current is not null)
        {
            if (!visited.Add(current.Value) || current.Value == departmentId)
            {
                return true;
            }

            current = parents.GetValueOrDefault(current.Value);
        }

        return false;
    }
}
