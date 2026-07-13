using HRManagement.Domain.Text;

namespace HRManagement.Domain.Tests.Text;

public sealed class PersianTextNormalizerTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("  علی   رضايي  ", "علی رضایی")]
    [InlineData("مي\u200cروم", "می روم")]
    [InlineData("كاظم", "کاظم")]
    [InlineData("۱۲۳٤٥", "12345")]
    public void Normalize_ProducesStableSearchText(string? input, string expected)
    {
        Assert.Equal(expected, PersianTextNormalizer.Normalize(input));
    }
}
