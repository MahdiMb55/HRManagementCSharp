using HRManagement.Domain.Entities;

namespace HRManagement.Domain.Tests.Entities;

public sealed class DepartmentTests
{
    [Fact]
    public void WouldCreateCycle_DetectsAncestorLoop()
    {
        IReadOnlyDictionary<long, long?> parents = new Dictionary<long, long?>
        {
            [1] = null,
            [2] = 3,
            [3] = 1,
        };

        Assert.True(Department.WouldCreateCycle(3, 2, parents));
        Assert.False(Department.WouldCreateCycle(3, 1, parents));
    }
}
