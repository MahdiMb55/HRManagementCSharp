namespace HRManagement.Application.Dashboard;

public sealed record DashboardSnapshot(
    int ActiveEmployees,
    int ArchivedOrDepartedEmployees,
    int ActiveContracts,
    IReadOnlyList<DepartmentEmployeeCount> EmployeesByDepartment);

public sealed record DepartmentEmployeeCount(string DepartmentName, int EmployeeCount);
