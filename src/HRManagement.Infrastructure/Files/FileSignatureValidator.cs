namespace HRManagement.Infrastructure.Files;

public static class FileSignatureValidator
{
    public static bool IsKnownSafeSignature(string extension, ReadOnlySpan<byte> header) =>
        extension.ToLowerInvariant() switch
        {
            ".pdf" => header.StartsWith("%PDF"u8),
            ".jpg" or ".jpeg" => header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => header.StartsWith(stackalloc byte[] { 0x89, 0x50, 0x4E, 0x47 }),
            ".doc" or ".docx" => true,
            _ => false,
        };

    public static string MimeTypeFor(string extension) =>
        extension.ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream",
        };
}
