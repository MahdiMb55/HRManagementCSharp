namespace HRManagement.Application.Files;

public interface IEmployeeFileService
{
    Task<FileRecordResult> CreateContractAsync(
        CreateContractRequest request,
        CancellationToken cancellationToken);

    Task<FileRecordResult> AddEmployeeDocumentAsync(
        AddEmployeeDocumentRequest request,
        CancellationToken cancellationToken);

    Task<FileRecordResult> SetProfilePhotoAsync(
        SetProfilePhotoRequest request,
        CancellationToken cancellationToken);
}
