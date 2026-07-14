using System.Reflection;
using System.Globalization;
using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees;
using HRManagement.Application.Employees.Search;
using HRManagement.Application.Employment;
using HRManagement.Application.Organization;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Employees;
using HRManagement.WinForms.Formatting;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeListControlTests
{
    [Fact]
    public void SetLoading_KeepsSearchInputEnabledSoInFlightSearchCanBeSuperseded()
    {
        using var control = CreateControl();

        control.SetLoading(true);

        Assert.True(GetPrivateControl<TextBox>(control, "searchTextBox").Enabled);
    }

    [Fact]
    public void ShowPage_RestoresNormalEmptyStateTextAfterError()
    {
        using var control = CreateControl();
        control.ShowError("خطا");

        control.ShowPage(new PagedResult<EmployeeListItemDto>([], 0, 1, 25));

        Assert.Equal("رکوردی برای نمایش وجود ندارد.", GetPrivateControl<Label>(control, "emptyStateLabel").Text);
    }

    [Fact]
    public void Dispose_CanBeCalledMoreThanOnce()
    {
        var control = CreateControl();

        control.Dispose();
        control.Dispose();
    }

    private static EmployeeListControl CreateControl() =>
        new(
            new StubSearchService(),
            new StubEditorService(),
            new StubEmploymentLifecycleService(),
            new StubAssignmentService(),
            new ImmediateDelay(),
            new InlineBackgroundExecutor(),
            new StubPersianDateAdapter(),
            NullLogger<EmployeeListPresenter>.Instance,
            NullLogger<EmployeeEditorPresenter>.Instance,
            NullLogger<EmploymentLifecycleForm>.Instance,
            NullLogger<OrganizationAssignmentsForm>.Instance);

    private static T GetPrivateControl<T>(EmployeeListControl control, string fieldName)
        where T : Control =>
        (T)(typeof(EmployeeListControl)
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(control)!);

    private sealed class StubSearchService : IEmployeeSearchService
    {
        public Task<PagedResult<EmployeeListItemDto>> SearchAsync(
            EmployeeSearchRequest request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<EmployeeListItemDto>([], 0, 1, 25));
    }

    private sealed class StubEditorService : IEmployeeEditorService
    {
        public Task<EmployeeEditDto?> GetAsync(long employeeId, CancellationToken cancellationToken) =>
            Task.FromResult<EmployeeEditDto?>(null);

        public Task<SaveEmployeeResult> SaveAsync(SaveEmployeeRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(SaveEmployeeResult.Success(1));
    }

    private sealed class StubEmploymentLifecycleService : IEmploymentLifecycleService
    {
        public Task<EmploymentLifecycleResult> StartEmploymentAsync(
            long employeeId,
            DateOnly hireDate,
            string? notes,
            CancellationToken cancellationToken) =>
            Task.FromResult(EmploymentLifecycleResult.Success(1));

        public Task<EmploymentLifecycleResult> TerminateAsync(
            long employeeId,
            DateOnly terminationDate,
            TerminationType terminationType,
            string reason,
            string? notes,
            CancellationToken cancellationToken) =>
            Task.FromResult(EmploymentLifecycleResult.Success(1));

        public Task<EmploymentLifecycleResult> ReturnToWorkAsync(
            long employeeId,
            DateOnly returnDate,
            string? notes,
            CancellationToken cancellationToken) =>
            Task.FromResult(EmploymentLifecycleResult.Success(1));

        public Task<EmploymentLifecycleResult> ChangeStatusAsync(
            long employeeId,
            EmploymentStatus status,
            DateOnly startDate,
            string? notes,
            CancellationToken cancellationToken) =>
            Task.FromResult(EmploymentLifecycleResult.Success(1));
    }

    private sealed class StubAssignmentService : IAssignmentService
    {
        public Task<OrganizationResult> AssignDepartmentAsync(
            long employeeId,
            long departmentId,
            DateOnly startDate,
            string? transferDescription,
            CancellationToken cancellationToken) =>
            Task.FromResult(OrganizationResult.Success(1));

        public Task<OrganizationResult> AssignResponsibilityAsync(
            long employeeId,
            long responsibilityId,
            DateOnly startDate,
            bool isPrimary,
            string? notes,
            CancellationToken cancellationToken) =>
            Task.FromResult(OrganizationResult.Success(1));

        public Task<OrganizationResult> EndResponsibilityAsync(
            long assignmentId,
            DateOnly endDate,
            long? newPrimaryAssignmentId,
            CancellationToken cancellationToken) =>
            Task.FromResult(OrganizationResult.Success(1));
    }

    private sealed class ImmediateDelay : IDelay
    {
        public Task DelayAsync(TimeSpan delay, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class InlineBackgroundExecutor : IBackgroundExecutor
    {
        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken) =>
            operation(cancellationToken);
    }

    private sealed class StubPersianDateAdapter : IPersianDateAdapter
    {
        public string Format(DateOnly? value) => value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

        public bool TryParse(string? value, out DateOnly result) =>
            DateOnly.TryParse(value, CultureInfo.InvariantCulture, out result);
    }
}
