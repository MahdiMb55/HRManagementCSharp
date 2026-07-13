using HRManagement.Domain.Identity;

namespace HRManagement.Domain.Tests.Identity;

public sealed class NationalCodeTests
{
    [Theory]
    [InlineData("0013548581")]
    [InlineData("۰۰۱۳۵۴۸۵۸۱")]
    public void Create_AcceptsValidIranianNationalCode(string input)
    {
        var result = NationalCode.Create(input);

        Assert.True(result.IsSuccess);
        Assert.Equal("0013548581", result.Value!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1111111111")]
    [InlineData("0013548589")]
    [InlineData("001354858x")]
    public void Create_RejectsMissingOrInvalidNationalCode(string? input)
    {
        var result = NationalCode.Create(input);

        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
    }
}
