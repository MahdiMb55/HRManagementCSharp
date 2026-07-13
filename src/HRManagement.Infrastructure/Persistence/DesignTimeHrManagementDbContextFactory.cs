using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HRManagement.Infrastructure.Persistence;

public sealed class DesignTimeHrManagementDbContextFactory
    : IDesignTimeDbContextFactory<HrManagementDbContext>
{
    public HrManagementDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HrManagementDbContext>()
            .UseSqlite("Data Source=hr-management-design.db;Foreign Keys=True")
            .Options;

        return new HrManagementDbContext(options);
    }
}
