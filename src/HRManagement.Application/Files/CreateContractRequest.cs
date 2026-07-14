using HRManagement.Domain.Enums;

namespace HRManagement.Application.Files;

public sealed record CreateContractRequest(
    long EmployeeId,
    string? ContractNumber,
    ContractType ContractType,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Notes,
    string? AttachmentSourcePath);
