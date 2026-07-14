namespace HRManagement.Application.Letters;

public sealed record IssueLetterResult(
    bool IsSuccess,
    string UserMessage,
    IReadOnlyList<string> OutputPaths)
{
    public static IssueLetterResult Success(IReadOnlyList<string> outputPaths) =>
        new(true, string.Empty, outputPaths);

    public static IssueLetterResult Failure(string userMessage) =>
        new(false, userMessage, []);
}
