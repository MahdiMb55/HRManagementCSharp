using HRManagement.Domain.Common;

namespace HRManagement.Domain.Entities;

public sealed class ContractAttachment : Entity
{
    private ContractAttachment()
    {
    }

    private ContractAttachment(long contractId, long managedFileId, string? description, DateTime createdAtUtc)
    {
        ContractId = contractId;
        ManagedFileId = managedFileId;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CreatedAtUtc = createdAtUtc.Kind == DateTimeKind.Utc ? createdAtUtc : createdAtUtc.ToUniversalTime();
    }

    public long ContractId { get; private set; }
    public long ManagedFileId { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ValidationResult<ContractAttachment> Create(
        long contractId,
        long managedFileId,
        string? description,
        DateTime createdAtUtc)
    {
        if (contractId <= 0 || managedFileId <= 0)
        {
            return ValidationResult<ContractAttachment>.Failure(
                "contract_attachment.required",
                "قرارداد و فایل پیوست الزامی است.");
        }

        return ValidationResult<ContractAttachment>.Success(
            new ContractAttachment(contractId, managedFileId, description, createdAtUtc));
    }
}
