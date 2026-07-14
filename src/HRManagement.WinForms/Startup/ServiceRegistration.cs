using HRManagement.Application.Abstractions;
using HRManagement.Application.Dashboard;
using HRManagement.Application.Employees;
using HRManagement.Application.Employees.Search;
using HRManagement.Infrastructure.Dashboard;
using HRManagement.Infrastructure.Employees;
using HRManagement.Infrastructure.Paths;
using HRManagement.Infrastructure.Persistence;
using HRManagement.Infrastructure.Threading;
using HRManagement.Infrastructure.Time;
using HRManagement.Infrastructure.Startup;
using HRManagement.WinForms.Controls;
using HRManagement.WinForms.Employees;
using HRManagement.WinForms.Formatting;
using HRManagement.WinForms.Forms;
using HRManagement.WinForms.Threading;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.WinForms.Startup;

public static class ServiceRegistration
{
    public static IServiceCollection AddHrManagement(
        this IServiceCollection services,
        ApplicationPaths paths)
    {
        services.AddSingleton(paths);
        services.AddSingleton<IApplicationPaths>(paths);
        services.AddPooledDbContextFactory<HrManagementDbContext>(options =>
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = paths.DatabaseFile,
                ForeignKeys = true,
                Pooling = true,
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();
            options.UseSqlite(connectionString);
        });

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IBackgroundExecutor, ThreadPoolBackgroundExecutor>();
        services.AddSingleton<IDelay, WinFormsDelay>();
        services.AddSingleton<IWritableDirectoryProbe, WritableDirectoryProbe>();
        services.AddSingleton<IStartupIntegrityService, StartupIntegrityService>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IEmployeeWriteRepository, EfEmployeeWriteRepository>();
        services.AddScoped<IEmployeeEditorService, EmployeeEditorService>();
        services.AddScoped<IEmployeeSearchRepository, EfEmployeeSearchRepository>();
        services.AddScoped<IEmployeeSearchService, EmployeeSearchService>();
        services.AddScoped<IDashboardService, EfDashboardService>();
        services.AddSingleton<IPersianDateAdapter, PersianDateAdapter>();
        services.AddSingleton<PrivateFontService>();
        services.AddSingleton<GlobalExceptionHandler>();
        services.AddTransient<DashboardControl>();
        services.AddTransient<EmployeeListControl>();
        services.AddTransient<MainForm>();
        return services;
    }
}
