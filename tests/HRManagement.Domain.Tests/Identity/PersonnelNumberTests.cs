using HRManagement.Domain.Identity;

namespace HRManagement.Domain.Tests.Identity;

public sealed class PersonnelNumberTests
{
    [Fact]
    public void Create_TrimsAndNormalizesPersianDigits()
    {
        var result = PersonnelNumber.Create("  ۰۰۱۲۳  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("00123", result.Value!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsMissingValue(string? input)
    {
        var result = PersonnelNumber.Create(input);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "personnel_number.required");
    }

    [Fact]
    public void Create_RejectsMoreThanFiftyCharacters()
    {
        var result = PersonnelNumber.Create(new string('1', 51));

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "personnel_number.too_long");
    }
}
