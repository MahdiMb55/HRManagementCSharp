using System.Net;
using System.Text;
using HRManagement.Application.Reports;

namespace HRManagement.Infrastructure.Reports;

public sealed class HtmlEmployeeSummaryService(
    IEmployeeSummaryRepository repository) : IEmployeeSummaryService
{
    public async Task<EmployeeSummaryResult> CreateAsync(
        EmployeeSummaryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.OutputPath))
        {
            return EmployeeSummaryResult.Failure("مسیر خروجی الزامی است.");
        }

        var rows = await repository.GetRowsAsync(request.EmployeeIds, cancellationToken);
        var directory = Path.GetDirectoryName(Path.GetFullPath(request.OutputPath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var html = BuildHtml(rows);
        await File.WriteAllTextAsync(request.OutputPath, html, new UTF8Encoding(true), cancellationToken);
        return EmployeeSummaryResult.Success(request.OutputPath);
    }

    private static string BuildHtml(IReadOnlyList<EmployeeSummaryRow> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<!doctype html><html lang=\"fa\" dir=\"rtl\"><head><meta charset=\"utf-8\"><title>خلاصه کارکنان</title>");
        builder.AppendLine("<style>body{font-family:Tahoma,Arial,sans-serif;margin:32px}table{border-collapse:collapse;width:100%}td,th{border:1px solid #ccc;padding:8px;text-align:right}th{background:#f3f3f3}</style>");
        builder.AppendLine("</head><body><h1>خلاصه کارکنان</h1><table><thead><tr><th>شماره پرسنلی</th><th>نام</th><th>کد ملی</th><th>تلفن</th><th>واحد</th><th>مسئولیت</th><th>وضعیت</th></tr></thead><tbody>");
        foreach (var row in rows)
        {
            builder.Append("<tr>");
            builder.Append(Cell(row.PersonnelNumber));
            builder.Append(Cell(row.FullName));
            builder.Append(Cell(row.NationalCode));
            builder.Append(Cell(row.MobileNumber));
            builder.Append(Cell(row.DepartmentName));
            builder.Append(Cell(row.PrimaryResponsibility));
            builder.Append(Cell(row.EmploymentStatus));
            builder.AppendLine("</tr>");
        }

        builder.AppendLine("</tbody></table></body></html>");
        return builder.ToString();
    }

    private static string Cell(string? value) =>
        "<td>" + WebUtility.HtmlEncode(value ?? string.Empty) + "</td>";
}
