using HRManagement.Application.Abstractions;

namespace HRManagement.Application.Organization;

public sealed class AssignmentService(
    IOrganizationRepository repository,
    IClock clock) : IAssignmentService
{
    public async Task<OrganizationResult> AssignDepartmentAsync(
        long employeeId,
        long departmentId,
        DateOnly startDate,
        string? transferDescription,
        CancellationToken cancellationToken)
    {
        var periodId = await repository.GetOpenEmploymentPeriodIdAsync(employeeId, cancellationToken);
        if (periodId is null)
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.EmploymentPeriodNotFound,
                "برای این کارمند دوره استخدام باز وجود ندارد.");
        }

        if (!await repository.ActiveDepartmentExistsAsync(departmentId, cancellationToken))
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.DepartmentNotFound,
                "واحد سازمانی فعال پیدا نشد.");
        }

        return await repository.AssignDepartmentAsync(
            employeeId,
            periodId.Value,
            departmentId,
            startDate,
            transferDescription,
            clock.UtcNow,
            cancellationToken);
    }

    public async Task<OrganizationResult> AssignResponsibilityAsync(
        long employeeId,
        long responsibilityId,
        DateOnly startDate,
        bool isPrimary,
        string? notes,
        CancellationToken cancellationToken)
    {
        var periodId = await repository.GetOpenEmploymentPeriodIdAsync(employeeId, cancellationToken);
        if (periodId is null)
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.EmploymentPeriodNotFound,
                "برای این کارمند دوره استخدام باز وجود ندارد.");
        }

        if (!await repository.ActiveResponsibilityExistsAsync(responsibilityId, cancellationToken))
        {
            return OrganizationResult.Failure(
                OrganizationFailureCode.ResponsibilityNotFound,
                "مسئولیت فعال پیدا نشد.");
        }

        return await repository.AssignResponsibilityAsync(
            employeeId,
            periodId.Value,
            responsibilityId,
            startDate,
            isPrimary,
            notes,
            clock.UtcNow,
            cancellationToken);
    }

    public Task<OrganizationResult> EndResponsibilityAsync(
        long assignmentId,
        DateOnly endDate,
        long? newPrimaryAssignmentId,
        CancellationToken cancellationToken) =>
        repository.EndResponsibilityAsync(assignmentId, endDate, newPrimaryAssignmentId, cancellationToken);
}
