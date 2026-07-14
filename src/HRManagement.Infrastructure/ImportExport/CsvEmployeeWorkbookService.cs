using System.Globalization;
using System.Text;
using HRManagement.Application.Employees.Search;
using HRManagement.Application.ImportExport;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Text;

namespace HRManagement.Infrastructure.ImportExport;

public sealed class CsvEmployeeWorkbookService(
    IEmployeeExportRepository repository) : IEmployeeWorkbookService
{
    private static readonly string[] Headers =
    [
        "PersonnelNumber",
        "FirstName",
        "LastName",
        "NationalCode",
        "MobileNumber",
        "DepartmentName",
        "PrimaryResponsibility",
        "EmploymentStatus",
    ];

    public async Task<ImportExportResult> CreateTemplateAsync(
        string outputPath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return ImportExportResult.Failure("مسیر خروجی الزامی است.");
        }

        EnsureParentDirectory(outputPath);
        await File.WriteAllTextAsync(outputPath, string.Join(",", Headers) + Environment.NewLine, new UTF8Encoding(true), cancellationToken);
        return ImportExportResult.Success(0, outputPath);
    }

    public async Task<EmployeeImportPreview> PreviewImportAsync(
        string inputPath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(inputPath) || !File.Exists(inputPath))
        {
            return new EmployeeImportPreview([], [new EmployeeImportRowError(0, "File", "فایل ورودی پیدا نشد.")]);
        }

        var rows = new List<EmployeeWorkbookRow>();
        var errors = new List<EmployeeImportRowError>();
        var lines = await File.ReadAllLinesAsync(inputPath, cancellationToken);
        for (var index = 1; index < lines.Length; index++)
        {
            var rowNumber = index + 1;
            var columns = SplitCsvLine(lines[index]);
            if (columns.Length < Headers.Length)
            {
                errors.Add(new EmployeeImportRowError(rowNumber, "Row", "تعداد ستون‌ها کامل نیست."));
                continue;
            }

            var personnelNumber = Normalize(columns[0]);
            var firstName = columns[1].Trim();
            var lastName = columns[2].Trim();
            var nationalCode = Normalize(columns[3]);
            if (string.IsNullOrWhiteSpace(personnelNumber))
            {
                errors.Add(new EmployeeImportRowError(rowNumber, "PersonnelNumber", "شماره پرسنلی الزامی است."));
            }

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(new EmployeeImportRowError(rowNumber, "Name", "نام و نام خانوادگی الزامی است."));
            }

            if (nationalCode.Length != 10)
            {
                errors.Add(new EmployeeImportRowError(rowNumber, "NationalCode", "کد ملی باید ۱۰ رقم باشد."));
            }

            rows.Add(new EmployeeWorkbookRow(
                null,
                personnelNumber,
                firstName,
                lastName,
                nationalCode,
                NormalizeOptional(columns[4]),
                NormalizeOptional(columns[5]),
                NormalizeOptional(columns[6]),
                string.IsNullOrWhiteSpace(columns[7]) ? EmploymentStatus.Active.ToString() : columns[7].Trim()));
        }

        return new EmployeeImportPreview(rows, errors);
    }

    public async Task<ImportExportResult> ExportAsync(
        EmployeeExportRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.OutputPath))
        {
            return ImportExportResult.Failure("مسیر خروجی الزامی است.");
        }

        var normalizedQuery = string.IsNullOrWhiteSpace(request.Query)
            ? null
            : PersianTextNormalizer.Normalize(request.Query);
        var rows = await repository.GetRowsAsync(
            normalizedQuery,
            request.Filter,
            request.Sort,
            request.SelectedEmployeeIds,
            cancellationToken);
        EnsureParentDirectory(request.OutputPath);
        await using var stream = File.Create(request.OutputPath);
        await using var writer = new StreamWriter(stream, new UTF8Encoding(true));
        await writer.WriteLineAsync(string.Join(",", Headers));
        foreach (var row in rows)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(string.Join(",", new[]
            {
                Csv(row.PersonnelNumber),
                Csv(row.FirstName),
                Csv(row.LastName),
                Csv(row.NationalCode),
                Csv(row.MobileNumber),
                Csv(row.DepartmentName),
                Csv(row.PrimaryResponsibility),
                Csv(row.EmploymentStatus),
            }));
        }

        return ImportExportResult.Success(rows.Count, request.OutputPath);
    }

    private static string Normalize(string value) =>
        PersianTextNormalizer.NormalizeDigits(value.Trim());

    private static string? NormalizeOptional(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : Normalize(value);

    private static string Csv(string? value)
    {
        var safe = value ?? string.Empty;
        return "\"" + safe.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }

    private static string[] SplitCsvLine(string line)
    {
        var values = new List<string>();
        var builder = new StringBuilder();
        var inQuotes = false;
        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];
            if (character == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    builder.Append('"');
                    index++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (character == ',' && !inQuotes)
            {
                values.Add(builder.ToString());
                builder.Clear();
            }
            else
            {
                builder.Append(character);
            }
        }

        values.Add(builder.ToString());
        return values.ToArray();
    }

    private static void EnsureParentDirectory(string path)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
