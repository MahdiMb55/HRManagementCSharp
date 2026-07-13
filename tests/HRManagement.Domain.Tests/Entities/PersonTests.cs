using HRManagement.Domain.Entities;
using HRManagement.Domain.Enums;
using HRManagement.Domain.Identity;

namespace HRManagement.Domain.Tests.Entities;

public sealed class PersonTests
{
    [Fact]
    public void Create_StoresValidatedIdentity()
    {
        var nationalCode = NationalCode.Create("0013548581").Value!;
        var now = new DateTime(2026, 7, 14, 8, 0, 0, DateTimeKind.Utc);

        var result = Person.Create("علی", "رضایی", nationalCode, Gender.Male, new DateOnly(1990, 1, 1), new DateOnly(2026, 7, 14), now);

        Assert.True(result.IsSuccess);
        Assert.Equal("0013548581", result.Value!.NationalCode);
        Assert.Equal(now, result.Value.CreatedAtUtc);
    }

    [Fact]
    public void Create_RejectsFutureBirthDate()
    {
        var nationalCode = NationalCode.Create("0013548581").Value!;

        var result = Person.Create("علی", "رضایی", nationalCode, Gender.Male, new DateOnly(2026, 7, 15), new DateOnly(2026, 7, 14), DateTime.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "person.birth_date.future");
    }
}
