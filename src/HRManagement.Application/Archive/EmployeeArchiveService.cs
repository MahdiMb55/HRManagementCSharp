using HRManagement.Application.Abstractions;

namespace HRManagement.Application.Archive;

public sealed class EmployeeArchiveService(
    IEmployeeArchiveRepository repository,
    IClock clock) : IEmployeeArchiveService
{
    public async Task<ArchiveResult> ArchiveAsync(long employeeId, CancellationToken cancellationToken)
    {
        var snapshot = await repository.FindAsync(employeeId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        if (snapshot.IsArchived)
        {
            return ArchiveResult.Failure(ArchiveFailureCode.AlreadyArchived, "کارمند قبلاً بایگانی شده است.");
        }

        return await repository.ArchiveAsync(employeeId, clock.UtcNow, cancellationToken);
    }

    public async Task<ArchiveResult> RestoreAsync(long employeeId, CancellationToken cancellationToken)
    {
        var snapshot = await repository.FindAsync(employeeId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        if (!snapshot.IsArchived)
        {
            return ArchiveResult.Failure(ArchiveFailureCode.NotArchived, "این کارمند در بایگانی نیست.");
        }

        return await repository.RestoreAsync(employeeId, clock.UtcNow, cancellationToken);
    }

    public async Task<ArchiveResult> DeletePermanentlyAsync(
        long employeeId,
        string? personnelNumberConfirmation,
        CancellationToken cancellationToken)
    {
        var snapshot = await repository.FindAsync(employeeId, cancellationToken);
        if (snapshot is null)
        {
            return NotFound();
        }

        if (!string.Equals(snapshot.PersonnelNumber, personnelNumberConfirmation?.Trim(), StringComparison.Ordinal))
        {
            return ArchiveResult.Failure(
                ArchiveFailureCode.ConfirmationMismatch,
                "برای حذف دائمی باید شماره پرسنلی را دقیقاً وارد کنید.");
        }

        return await repository.DeletePermanentlyAsync(employeeId, clock.UtcNow, cancellationToken);
    }

    private static ArchiveResult NotFound() =>
        ArchiveResult.Failure(ArchiveFailureCode.EmployeeNotFound, "کارمند موردنظر پیدا نشد.");
}
