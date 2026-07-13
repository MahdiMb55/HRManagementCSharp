using HRManagement.Domain.Entities;
using HRManagement.Domain.Identity;

namespace HRManagement.Domain.Tests.Entities;

public sealed class EmployeeTests
{
    [Fact]
    public void Create_StoresPersonnelNumberAndStartsActive()
    {
        var number = PersonnelNumber.Create("0012").Value!;

        var result = Employee.Create(7, number, DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.Equal("0012", result.Value!.PersonnelNumber);
        Assert.False(result.Value.IsDeleted);
    }
}
