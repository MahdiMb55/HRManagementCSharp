using HRManagement.Domain.Entities;

namespace HRManagement.Domain.Tests.Entities;

public sealed class EmploymentPeriodTests
{
    [Fact]
    public void End_RejectsDateBeforeHireDate()
    {
        var period = EmploymentPeriod.Create(10, new DateOnly(2020, 1, 1), null, DateTime.UtcNow).Value!;

        var result = period.End(new DateOnly(2019, 12, 31), DateTime.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Null(period.EndDate);
    }

    [Fact]
    public void End_ClosesOpenPeriod()
    {
        var period = EmploymentPeriod.Create(10, new DateOnly(2020, 1, 1), null, DateTime.UtcNow).Value!;

        var result = period.End(new DateOnly(2024, 1, 1), DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2024, 1, 1), period.EndDate);
    }
}
