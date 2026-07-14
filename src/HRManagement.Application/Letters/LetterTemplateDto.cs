namespace HRManagement.Application.Letters;

public sealed record LetterTemplateDto(
    long TemplateId,
    string Title,
    string? Description);
