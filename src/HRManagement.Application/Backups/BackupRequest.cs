using HRManagement.Domain.Enums;

namespace HRManagement.Application.Backups;

public sealed record BackupRequest(
    BackupType BackupType,
    string? DestinationDirectory);
