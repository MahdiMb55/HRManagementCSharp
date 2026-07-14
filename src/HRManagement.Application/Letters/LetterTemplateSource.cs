namespace HRManagement.Application.Letters;

public sealed record LetterTemplateSource(
    long TemplateId,
    string Title,
    string RelativePath);
