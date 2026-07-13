namespace HRManagement.Infrastructure.Persistence;

public sealed record SystemDocumentCategorySeed(long Id, string Name, string Description);

public static class SystemDocumentCategories
{
    public static readonly DateTime SeedTimestampUtc =
        new(2026, 7, 14, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<SystemDocumentCategorySeed> All { get; } =
    [
        new(1, "کارت ملی", "مدارک کارت ملی"),
        new(2, "شناسنامه", "مدارک شناسنامه"),
        new(3, "تحصیلات", "مدارک تحصیلی"),
        new(4, "قرارداد", "مدارک قرارداد"),
        new(5, "بیمه", "مدارک بیمه"),
        new(6, "نظام وظیفه", "مدارک نظام وظیفه"),
        new(7, "سایر", "سایر مدارک"),
    ];
}
