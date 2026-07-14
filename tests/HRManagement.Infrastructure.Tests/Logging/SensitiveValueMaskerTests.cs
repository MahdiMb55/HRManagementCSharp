using HRManagement.Infrastructure.Logging;

namespace HRManagement.Infrastructure.Tests.Logging;

public sealed class SensitiveValueMaskerTests
{
    [Theory]
    [InlineData("0013548581", "******8581")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void MaskNationalCode_RevealsOnlyLastFour(string? value, string? expected)
    {
        Assert.Equal(expected, SensitiveValueMasker.MaskNationalCode(value));
    }

    [Theory]
    [InlineData("6037991234567890", "************7890")]
    [InlineData("IR820540102680020817909002", "**********************9002")]
    public void MaskFinancialIdentifier_RevealsOnlyLastFour(string value, string expected)
    {
        Assert.Equal(expected, SensitiveValueMasker.MaskFinancialIdentifier(value));
    }
}
