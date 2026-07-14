using HRManagement.WinForms.Employees;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeListShortcutTests
{
    [Theory]
    [InlineData(Keys.Control | Keys.N, EmployeeListShortcutCommand.Add)]
    [InlineData(Keys.Control | Keys.F, EmployeeListShortcutCommand.FocusSearch)]
    [InlineData(Keys.Enter, EmployeeListShortcutCommand.OpenSelected)]
    public void Resolve_MapsApprovedEmployeeListShortcuts(
        Keys keyData,
        EmployeeListShortcutCommand expected)
    {
        Assert.Equal(expected, EmployeeListShortcut.Resolve(keyData));
    }

    [Fact]
    public void Resolve_IgnoresUnrelatedKey()
    {
        Assert.Equal(EmployeeListShortcutCommand.None, EmployeeListShortcut.Resolve(Keys.Escape));
    }
}
