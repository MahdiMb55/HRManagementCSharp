namespace HRManagement.Application.Letters;

public interface ILetterService
{
    Task<IReadOnlyList<LetterTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken);

    Task<IssueLetterResult> RegisterTemplateAsync(
        RegisterLetterTemplateRequest request,
        CancellationToken cancellationToken);

    Task<IssueLetterResult> IssueAsync(
        IssueLetterRequest request,
        CancellationToken cancellationToken);
}
