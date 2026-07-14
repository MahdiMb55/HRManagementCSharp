using HRManagement.WinForms.Formatting;

namespace HRManagement.Application.Tests.Employees;

public sealed class PersianDateAdapterTests
{
    [Fact]
    public void FormatAndTryParse_RoundTripsJalaliDate()
    {
        var adapter = new PersianDateAdapter();
        var gregorian = new DateOnly(2026, 3, 21);

        var formatted = adapter.Format(gregorian);
        var parsed = adapter.TryParse(formatted, out var result);

        Assert.Equal("1405/01/01", formatted);
        Assert.True(parsed);
        Assert.Equal(gregorian, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1405/13/01")]
    [InlineData("متن")]
    public void TryParse_RejectsInvalidValue(string value)
    {
        Assert.False(new PersianDateAdapter().TryParse(value, out _));
    }
}
