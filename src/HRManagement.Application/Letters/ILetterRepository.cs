using HRManagement.Domain.Entities;

namespace HRManagement.Application.Letters;

public interface ILetterRepository
{
    Task<IReadOnlyList<LetterTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken);

    Task<LetterTemplateSource?> GetTemplateSourceAsync(long templateId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EmployeeLetterProjection>> GetEmployeeProjectionsAsync(
        IReadOnlyCollection<long> employeeIds,
        CancellationToken cancellationToken);

    Task<long> RegisterTemplateAsync(
        string title,
        string? description,
        ManagedFile file,
        DateTime nowUtc,
        CancellationToken cancellationToken);

    Task SaveIssuedLetterAsync(
        long employeeId,
        long templateId,
        string letterNumber,
        DateOnly issueDate,
        string? subject,
        ManagedFile outputFile,
        DateTime nowUtc,
        CancellationToken cancellationToken);
}
