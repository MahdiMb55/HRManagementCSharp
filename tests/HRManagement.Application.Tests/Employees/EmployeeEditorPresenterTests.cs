using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees;
using HRManagement.Domain.Enums;
using HRManagement.WinForms.Employees;
using Microsoft.Extensions.Logging;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeEditorPresenterTests
{
    [Fact]
    public async Task SaveAsync_ShowsFieldErrorForInvalidNationalCode()
    {
        var view = new FakeEditorView { ThrowOnGeneralError = false };
        var service = new StubEditorService(
            SaveEmployeeResult.Failure(EmployeeSaveFailureCode.InvalidNationalCode, "کد ملی واردشده معتبر نیست."));
        var presenter = new EmployeeEditorPresenter(view, service);

        await presenter.SaveAsync(CancellationToken.None);

        Assert.Equal("کد ملی واردشده معتبر نیست.", view.Errors[EmployeeEditorField.NationalCode]);
        Assert.False(view.Saved);
    }

    [Fact]
    public async Task SaveAsync_NotifiesViewAfterSuccessfulCommit()
    {
        var view = new FakeEditorView();
        var presenter = new EmployeeEditorPresenter(view, new StubEditorService(SaveEmployeeResult.Success(7)));

        await presenter.SaveAsync(CancellationToken.None);

        Assert.True(view.Saved);
        Assert.Equal(7, view.SavedEmployeeId);
        Assert.False(view.IsBusy);
    }

    [Fact]
    public async Task SaveAsync_LogsUnexpectedServiceFailure()
    {
        var view = new FakeEditorView { ThrowOnGeneralError = false };
        var logger = new CapturingLogger<EmployeeEditorPresenter>();
        var presenter = new EmployeeEditorPresenter(view, new ThrowingEditorService(), logger);

        await presenter.SaveAsync(CancellationToken.None);

        Assert.Single(logger.Exceptions);
        Assert.NotNull(view.GeneralError);
    }

    [Fact]
    public async Task SaveAsync_RunsSaveThroughBackgroundExecutor()
    {
        var view = new FakeEditorView();
        var executor = new CapturingBackgroundExecutor();
        var presenter = new EmployeeEditorPresenter(
            view,
            new StubEditorService(SaveEmployeeResult.Success(7)),
            backgroundExecutor: executor);

        await presenter.SaveAsync(CancellationToken.None);

        Assert.True(executor.WasUsed);
        Assert.True(view.Saved);
    }

    [Fact]
    public async Task LoadAsync_ReturnsFalseWhenEmployeeDoesNotExist()
    {
        var view = new FakeEditorView { ThrowOnGeneralError = false };
        var presenter = new EmployeeEditorPresenter(view, new StubEditorService(SaveEmployeeResult.Success(1)));

        var loaded = await presenter.LoadAsync(42, CancellationToken.None);

        Assert.False(loaded);
        Assert.NotNull(view.GeneralError);
    }

    [Fact]
    public async Task LoadAsync_RunsLookupThroughBackgroundExecutor()
    {
        var view = new FakeEditorView();
        var executor = new CapturingBackgroundExecutor();
        var service = new StubEditorService(SaveEmployeeResult.Success(1))
        {
            Existing = new EmployeeEditDto(9, "علی", "رضایی", "0099", "0013548581", null, Gender.Male, null, null),
        };
        var presenter = new EmployeeEditorPresenter(view, service, backgroundExecutor: executor);

        var loaded = await presenter.LoadAsync(9, CancellationToken.None);

        Assert.True(loaded);
        Assert.True(executor.WasUsed);
        Assert.NotNull(view.AppliedEmployee);
    }

    private sealed class FakeEditorView : IEmployeeEditorView
    {
        public long? EmployeeId => null;
        public string FirstName => "علی";
        public string LastName => "رضایی";
        public string PersonnelNumber => "0012";
        public string NationalCode => "0013548581";
        public string? FatherName => null;
        public Gender Gender => Gender.Male;
        public DateOnly? BirthDate => null;
        public string? MobileNumber => null;
        public bool IsBusy { get; private set; }
        public bool Saved { get; private set; }
        public long SavedEmployeeId { get; private set; }
        public Dictionary<EmployeeEditorField, string> Errors { get; } = [];
        public bool ThrowOnGeneralError { get; init; } = true;
        public string? GeneralError { get; private set; }
        public EmployeeEditDto? AppliedEmployee { get; private set; }

        public void Apply(EmployeeEditDto employee) => AppliedEmployee = employee;
        public void ClearErrors() => Errors.Clear();
        public void SetBusy(bool isBusy) => IsBusy = isBusy;
        public void ShowFieldError(EmployeeEditorField field, string message) => Errors[field] = message;
        public void ShowGeneralError(string message)
        {
            GeneralError = message;
            if (ThrowOnGeneralError)
            {
                throw new Xunit.Sdk.XunitException(message);
            }
        }
        public void NotifySaved(long employeeId)
        {
            Saved = true;
            SavedEmployeeId = employeeId;
        }
    }

    private sealed class StubEditorService(SaveEmployeeResult result) : IEmployeeEditorService
    {
        public EmployeeEditDto? Existing { get; init; }

        public Task<EmployeeEditDto?> GetAsync(long employeeId, CancellationToken cancellationToken) =>
            Task.FromResult(Existing);

        public Task<SaveEmployeeResult> SaveAsync(SaveEmployeeRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(result);
    }

    private sealed class ThrowingEditorService : IEmployeeEditorService
    {
        public Task<EmployeeEditDto?> GetAsync(long employeeId, CancellationToken cancellationToken) =>
            Task.FromException<EmployeeEditDto?>(new InvalidOperationException("database failure"));

        public Task<SaveEmployeeResult> SaveAsync(
            SaveEmployeeRequest request,
            CancellationToken cancellationToken) =>
            Task.FromException<SaveEmployeeResult>(new InvalidOperationException("database failure"));
    }

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<Exception?> Exceptions { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => EmptyScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) => Exceptions.Add(exception);

        private sealed class EmptyScope : IDisposable
        {
            public static EmptyScope Instance { get; } = new();
            public void Dispose() { }
        }
    }

    private sealed class CapturingBackgroundExecutor : IBackgroundExecutor
    {
        public bool WasUsed { get; private set; }

        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
        {
            WasUsed = true;
            return operation(cancellationToken);
        }
    }
}
