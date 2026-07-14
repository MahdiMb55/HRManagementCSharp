namespace HRManagement.WinForms.Employees;

public enum EmployeeListShortcutCommand
{
    None = 0,
    Add = 1,
    FocusSearch = 2,
    OpenSelected = 3,
}

public static class EmployeeListShortcut
{
    public static EmployeeListShortcutCommand Resolve(Keys keyData) => keyData switch
    {
        Keys.Control | Keys.N => EmployeeListShortcutCommand.Add,
        Keys.Control | Keys.F => EmployeeListShortcutCommand.FocusSearch,
        Keys.Enter => EmployeeListShortcutCommand.OpenSelected,
        _ => EmployeeListShortcutCommand.None,
    };
}
