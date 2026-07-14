namespace HRManagement.Application.Organization;

public interface IAssignmentService
{
    Task<OrganizationResult> AssignDepartmentAsync(
        long employeeId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        CancellationToken cancellationToken);

    Task<OrganizationResult> AssignResponsibilityAsync(
        long employeeId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        CancellationToken cancellationToken);

    Task<OrganizationResult> EndResponsibilityAsync(
        long assignmentId,
        DateOnly endDate,
        long? newPrimaryAssignmentId,
        CancellationToken cancellationToken);
}
