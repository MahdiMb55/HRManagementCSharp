using HRManagement.Domain.Common;
using System.Globalization;

namespace HRManagement.Domain.Tests.Common;

public sealed class DateRangeTests
{
    [Fact]
    public void Create_RejectsEndBeforeStart()
    {
        var result = DateRange.Create(new DateOnly(2026, 2, 2), new DateOnly(2026, 2, 1));

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "date_range.invalid_order");
    }

    [Theory]
    [InlineData("2026-01-01", "2026-01-10", "2026-01-10", "2026-01-20", true)]
    [InlineData("2026-01-01", "2026-01-09", "2026-01-10", "2026-01-20", false)]
    [InlineData("2026-01-01", null, "2030-01-01", null, true)]
    public void Overlaps_UsesInclusiveBoundaries(
        string firstStart,
        string? firstEnd,
        string secondStart,
        string? secondEnd,
        bool expected)
    {
        var first = DateRange.Create(DateOnly.Parse(firstStart, CultureInfo.InvariantCulture), Parse(firstEnd)).Value!;
        var second = DateRange.Create(DateOnly.Parse(secondStart, CultureInfo.InvariantCulture), Parse(secondEnd)).Value!;

        Assert.Equal(expected, first.Overlaps(second));
    }

    private static DateOnly? Parse(string? value) =>
        value is null ? null : DateOnly.Parse(value, CultureInfo.InvariantCulture);
}
