using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using HRManagement.Application.Abstractions;
using HRManagement.Application.Letters;
using HRManagement.Domain.Entities;

namespace HRManagement.Infrastructure.Letters;

public sealed class OpenXmlLetterService(
    IApplicationPaths paths,
    ILetterRepository repository,
    IClock clock) : ILetterService
{
    public Task<IReadOnlyList<LetterTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken) =>
        repository.GetTemplatesAsync(cancellationToken);

    public async Task<IssueLetterResult> RegisterTemplateAsync(
        RegisterLetterTemplateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.SourcePath) ||
            !File.Exists(request.SourcePath))
        {
            return IssueLetterResult.Failure("عنوان و فایل قالب الزامی است.");
        }

        if (!string.Equals(Path.GetExtension(request.SourcePath), ".docx", StringComparison.OrdinalIgnoreCase))
        {
            return IssueLetterResult.Failure("فقط قالب DOCX پشتیبانی می‌شود.");
        }

        Directory.CreateDirectory(paths.LetterTemplatesDirectory);
        var nowUtc = clock.UtcNow;
        var storedFileName = $"{nowUtc:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.docx";
        var target = Path.Combine(paths.LetterTemplatesDirectory, storedFileName);
        File.Copy(request.SourcePath, target, overwrite: false);
        var relativePath = Path.GetRelativePath(paths.DataDirectory, target).Replace('\\', '/');
        var fileInfo = new FileInfo(target);
        var file = ManagedFile.Create(
            Path.GetFileName(request.SourcePath),
            storedFileName,
            relativePath,
            ".docx",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            fileInfo.Length,
            await Sha256Async(target, cancellationToken),
            nowUtc);
        if (!file.IsSuccess)
        {
            return IssueLetterResult.Failure(file.Errors[0].Message);
        }

        var templateId = await repository.RegisterTemplateAsync(request.Title, request.Description, file.Value!, nowUtc, cancellationToken);
        return IssueLetterResult.Success([$"template:{templateId}"]);
    }

    public async Task<IssueLetterResult> IssueAsync(
        IssueLetterRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TemplateId <= 0 || request.EmployeeIds.Count == 0 || string.IsNullOrWhiteSpace(request.LetterNumber))
        {
            return IssueLetterResult.Failure("قالب، کارمند و شماره نامه الزامی است.");
        }

        var template = await repository.GetTemplateSourceAsync(request.TemplateId, cancellationToken);
        if (template is null)
        {
            return IssueLetterResult.Failure("قالب نامه پیدا نشد.");
        }

        var templatePath = Path.Combine(paths.DataDirectory, template.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(templatePath))
        {
            return IssueLetterResult.Failure("فایل قالب نامه پیدا نشد.");
        }

        var employees = await repository.GetEmployeeProjectionsAsync(request.EmployeeIds, cancellationToken);
        if (employees.Count == 0)
        {
            return IssueLetterResult.Failure("کارمند معتبری برای صدور نامه پیدا نشد.");
        }

        Directory.CreateDirectory(paths.GeneratedLettersDirectory);
        var outputPaths = new List<string>();
        foreach (var employee in employees)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var outputFileName = $"{Sanitize(request.LetterNumber)}-{employee.PersonnelNumber}-{Guid.NewGuid():N}.docx";
            var outputPath = Path.Combine(paths.GeneratedLettersDirectory, outputFileName);
            File.Copy(templatePath, outputPath, overwrite: false);
            ReplacePlaceholders(outputPath, BuildPlaceholders(employee, request));
            var nowUtc = clock.UtcNow;
            var relativePath = Path.GetRelativePath(paths.DataDirectory, outputPath).Replace('\\', '/');
            var info = new FileInfo(outputPath);
            var managedOutput = ManagedFile.Create(
                outputFileName,
                outputFileName,
                relativePath,
                ".docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                info.Length,
                await Sha256Async(outputPath, cancellationToken),
                nowUtc);
            if (!managedOutput.IsSuccess)
            {
                return IssueLetterResult.Failure(managedOutput.Errors[0].Message);
            }

            await repository.SaveIssuedLetterAsync(
                employee.EmployeeId,
                request.TemplateId,
                request.LetterNumber,
                request.IssueDate,
                request.Subject,
                managedOutput.Value!,
                nowUtc,
                cancellationToken);
            outputPaths.Add(outputPath);
        }

        return IssueLetterResult.Success(outputPaths);
    }

    private static LetterPlaceholderSet BuildPlaceholders(
        EmployeeLetterProjection employee,
        IssueLetterRequest request) =>
        new(new Dictionary<string, string>
        {
            ["Employee.FullName"] = $"{employee.FirstName} {employee.LastName}",
            ["Employee.FirstName"] = employee.FirstName,
            ["Employee.LastName"] = employee.LastName,
            ["Employee.PersonnelNumber"] = employee.PersonnelNumber,
            ["Employee.NationalCode"] = employee.NationalCode,
            ["Employee.MobileNumber"] = employee.MobileNumber ?? string.Empty,
            ["Employee.Department"] = employee.DepartmentName ?? string.Empty,
            ["Employee.Responsibility"] = employee.PrimaryResponsibility ?? string.Empty,
            ["Company.Name"] = employee.CompanyName ?? string.Empty,
            ["Letter.Number"] = request.LetterNumber,
            ["Letter.Date"] = request.IssueDate.ToString("yyyy/MM/dd"),
            ["Letter.Subject"] = request.Subject ?? string.Empty,
        });

    private static void ReplacePlaceholders(string docxPath, LetterPlaceholderSet placeholders)
    {
        using var archive = ZipFile.Open(docxPath, ZipArchiveMode.Update);
        foreach (var entry in archive.Entries.Where(entry => entry.FullName.StartsWith("word/", StringComparison.OrdinalIgnoreCase) && entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
        {
            string xml;
            using (var reader = new StreamReader(entry.Open()))
            {
                xml = reader.ReadToEnd();
            }

            var updated = xml;
            foreach (var placeholder in placeholders.Values)
            {
                updated = updated.Replace("{{" + placeholder.Key + "}}", EscapeXmlText(placeholder.Value), StringComparison.Ordinal);
            }

            if (string.Equals(xml, updated, StringComparison.Ordinal))
            {
                continue;
            }

            using var stream = entry.Open();
            stream.SetLength(0);
            using var writer = new StreamWriter(stream);
            writer.Write(updated);
        }
    }

    private static string EscapeXmlText(string value) =>
        value.Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

    private static string Sanitize(string value) =>
        Regex.Replace(value, "[^A-Za-z0-9\\-_]+", "-");

    private static async Task<string> Sha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
