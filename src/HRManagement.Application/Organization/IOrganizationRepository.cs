namespace HRManagement.Application.Organization;

public interface IOrganizationRepository
{
    Task<long?> GetOpenEmploymentPeriodIdAsync(long employeeId, CancellationToken cancellationToken);
    Task<bool> ActiveDepartmentExistsAsync(long departmentId, CancellationToken cancellationToken);
    Task<bool> ActiveResponsibilityExistsAsync(long responsibilityId, CancellationToken cancellationToken);

    Task<OrganizationResult> AssignDepartmentAsync(
        long employeeId,
        long employmentPeriodId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<OrganizationResult> AssignResponsibilityAsync(
        long employeeId,
        long employmentPeriodId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task<OrganizationResult> EndResponsibilityAsync(
        long assignmentId,
        DateOnly endDate,
        long? newPrimaryAssignmentId,
        CancellationToken cancellationToken);
}
