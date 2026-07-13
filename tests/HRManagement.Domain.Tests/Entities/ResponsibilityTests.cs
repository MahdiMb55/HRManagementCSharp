using HRManagement.Domain.Entities;

namespace HRManagement.Domain.Tests.Entities;

public sealed class ResponsibilityTests
{
    [Fact]
    public void Create_TrimsTitleAndStartsActive()
    {
        var result = Responsibility.Create("  کارشناس منابع انسانی  ", DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal("کارشناس منابع انسانی", result.Value!.Title);
        Assert.True(result.Value.IsActive);
    }
}
