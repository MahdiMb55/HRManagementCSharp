namespace HRManagement.Application.Letters;

public sealed record LetterPlaceholderSet(
    IReadOnlyDictionary<string, string> Values)
{
    public static IReadOnlyList<string> ApprovedPlaceholders { get; } =
    [
        "Employee.FullName",
        "Employee.FirstName",
        "Employee.LastName",
        "Employee.PersonnelNumber",
        "Employee.NationalCode",
        "Employee.MobileNumber",
        "Employee.Department",
        "Employee.Responsibility",
        "Company.Name",
        "Letter.Number",
        "Letter.Date",
        "Letter.Subject",
    ];
}
