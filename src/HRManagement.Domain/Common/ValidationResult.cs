namespace HRManagement.Domain.Common;

public sealed class ValidationResult<T>
{
    private ValidationResult(T? value, IReadOnlyList<ValidationError> errors)
    {
        Value = value;
        Errors = errors;
    }

    public bool IsSuccess => Errors.Count == 0;

    public T? Value { get; }

    public IReadOnlyList<ValidationError> Errors { get; }

    public static ValidationResult<T> Success(T value) =>
        new(value, Array.Empty<ValidationError>());

    public static ValidationResult<T> Failure(string code, string message) =>
        new(default, [new ValidationError(code, message)]);
}
