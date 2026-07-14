using HRManagement.Application.Abstractions;
using HRManagement.Domain.Entities;

namespace HRManagement.Application.Files;

public sealed class EmployeeFileService(
    IEmployeeFileRepository repository,
    IManagedFileStore fileStore,
    IClock clock) : IEmployeeFileService
{
    private static readonly IReadOnlySet<string> ContractExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".doc",
        ".docx",
        ".jpg",
        ".jpeg",
        ".png",
    };

    private static readonly IReadOnlySet<string> DocumentExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png",
        ".doc",
        ".docx",
    };

    private static readonly IReadOnlySet<string> PhotoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
    };

    public async Task<FileRecordResult> CreateContractAsync(
        CreateContractRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return NotFound();
        }

        var employmentPeriodId = await repository.GetOpenEmploymentPeriodIdAsync(request.EmployeeId, cancellationToken);
        if (employmentPeriodId is null)
        {
            return FileRecordResult.Failure(
                FileRecordFailureCode.EmploymentPeriodRequired,
                "برای ثبت قرارداد، دوره استخدام فعال لازم است.");
        }

        var nowUtc = clock.UtcNow;
        ManagedFile? attachment = null;
        if (!string.IsNullOrWhiteSpace(request.AttachmentSourcePath))
        {
            var file = await fileStore.SaveAsync(
                new ManagedFileSaveRequest(request.AttachmentSourcePath, ManagedFileKind.ContractAttachment, ContractExtensions),
                nowUtc,
                cancellationToken);
            if (!file.IsSuccess)
            {
                return FileRecordResult.Failure(FileRecordFailureCode.FileRejected, file.UserMessage);
            }

            attachment = file.File;
        }

        var contract = Contract.Create(
            request.EmployeeId,
            employmentPeriodId.Value,
            request.ContractNumber,
            request.ContractType,
            request.StartDate,
            request.EndDate,
            request.Notes,
            nowUtc);
        if (!contract.IsSuccess)
        {
            return Invalid(contract.Errors[0].Message);
        }

        return await repository.CreateContractAsync(contract.Value!, attachment, request.Notes, nowUtc, cancellationToken);
    }

    public async Task<FileRecordResult> AddEmployeeDocumentAsync(
        AddEmployeeDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return NotFound();
        }

        var nowUtc = clock.UtcNow;
        var file = await fileStore.SaveAsync(
            new ManagedFileSaveRequest(request.SourcePath, ManagedFileKind.Document, DocumentExtensions),
            nowUtc,
            cancellationToken);
        if (!file.IsSuccess)
        {
            return FileRecordResult.Failure(FileRecordFailureCode.FileRejected, file.UserMessage);
        }

        var document = EmployeeDocument.Create(
            request.EmployeeId,
            request.CategoryId,
            request.Title,
            request.Description,
            nowUtc);
        if (!document.IsSuccess)
        {
            return Invalid(document.Errors[0].Message);
        }

        return await repository.AddEmployeeDocumentVersionAsync(document.Value!, file.File!, nowUtc, cancellationToken);
    }

    public async Task<FileRecordResult> SetProfilePhotoAsync(
        SetProfilePhotoRequest request,
        CancellationToken cancellationToken)
    {
        if (!await repository.EmployeeExistsAsync(request.EmployeeId, cancellationToken))
        {
            return NotFound();
        }

        var nowUtc = clock.UtcNow;
        var file = await fileStore.SaveAsync(
            new ManagedFileSaveRequest(request.SourcePath, ManagedFileKind.Photo, PhotoExtensions),
            nowUtc,
            cancellationToken);
        if (!file.IsSuccess)
        {
            return FileRecordResult.Failure(FileRecordFailureCode.FileRejected, file.UserMessage);
        }

        return await repository.SetProfilePhotoAsync(request.EmployeeId, file.File!, nowUtc, cancellationToken);
    }

    private static FileRecordResult Invalid(string message) =>
        FileRecordResult.Failure(FileRecordFailureCode.InvalidInput, message);

    private static FileRecordResult NotFound() =>
        FileRecordResult.Failure(FileRecordFailureCode.EmployeeNotFound, "کارمند موردنظر پیدا نشد.");
}
