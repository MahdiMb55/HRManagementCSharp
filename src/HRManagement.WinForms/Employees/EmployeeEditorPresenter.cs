using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRManagement.WinForms.Employees;

public sealed class EmployeeEditorPresenter(
    IEmployeeEditorView view,
    IEmployeeEditorService editorService,
    ILogger<EmployeeEditorPresenter>? logger = null,
    IBackgroundExecutor? backgroundExecutor = null)
{
    public async Task<bool> LoadAsync(long employeeId, CancellationToken cancellationToken)
    {
        view.SetBusy(true);
        try
        {
            var executor = backgroundExecutor ?? new InlineBackgroundExecutor();
            var employee = await executor.ExecuteAsync(
                backgroundToken => editorService.GetAsync(employeeId, backgroundToken),
                cancellationToken);
            if (employee is null)
            {
                view.ShowGeneralError("کارمند موردنظر پیدا نشد.");
                return false;
            }

            view.Apply(employee);
            return true;
        }
        finally
        {
            view.SetBusy(false);
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        view.ClearErrors();
        view.SetBusy(true);
        try
        {
            var request = new SaveEmployeeRequest(
                view.EmployeeId,
                view.FirstName,
                view.LastName,
                view.PersonnelNumber,
                view.NationalCode,
                view.FatherName,
                view.Gender,
                view.BirthDate,
                view.MobileNumber);
            var executor = backgroundExecutor ?? new InlineBackgroundExecutor();
            var result = await executor.ExecuteAsync(
                backgroundToken => editorService.SaveAsync(request, backgroundToken),
                cancellationToken);
            if (result.IsSuccess && result.EmployeeId is long employeeId)
            {
                view.NotifySaved(employeeId);
                return;
            }

            var field = result.FailureCode switch
            {
                EmployeeSaveFailureCode.RequiredName => EmployeeEditorField.FirstName,
                EmployeeSaveFailureCode.InvalidNationalCode or EmployeeSaveFailureCode.DuplicateNationalCode => EmployeeEditorField.NationalCode,
                EmployeeSaveFailureCode.InvalidPersonnelNumber or
                EmployeeSaveFailureCode.DuplicatePersonnelNumber or
                EmployeeSaveFailureCode.PersonnelNumberChangeRequiresReason => EmployeeEditorField.PersonnelNumber,
                EmployeeSaveFailureCode.FutureBirthDate => EmployeeEditorField.BirthDate,
                _ => EmployeeEditorField.General,
            };
            if (field == EmployeeEditorField.General)
            {
                view.ShowGeneralError(result.UserMessage);
            }
            else
            {
                view.ShowFieldError(field, result.UserMessage);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception exception)
        {
            (logger ?? NullLogger<EmployeeEditorPresenter>.Instance)
                .LogError(exception, "Employee save failed");
            view.ShowGeneralError("ذخیره اطلاعات انجام نشد. جزئیات خطا در گزارش برنامه ثبت شد.");
        }
        finally
        {
            view.SetBusy(false);
        }
    }

    private sealed class InlineBackgroundExecutor : IBackgroundExecutor
    {
        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken) =>
            operation(cancellationToken);
    }
}
