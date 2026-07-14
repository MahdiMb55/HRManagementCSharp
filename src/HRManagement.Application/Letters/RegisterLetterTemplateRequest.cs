namespace HRManagement.Application.Letters;

public sealed record RegisterLetterTemplateRequest(
    string Title,
    string? Description,
    string SourcePath);
