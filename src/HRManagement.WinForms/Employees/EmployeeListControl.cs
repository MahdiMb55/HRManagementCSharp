using HRManagement.Application.Abstractions;
using HRManagement.Application.Archive;
using HRManagement.Application.Employees;
using HRManagement.Application.Employees.Search;
using HRManagement.Application.Employment;
using HRManagement.Application.Files;
using HRManagement.Application.ImportExport;
using HRManagement.Application.Organization;
using HRManagement.Application.PersonnelRecords;
using HRManagement.Application.Reports;
using HRManagement.WinForms.Formatting;
using HRManagement.WinForms.ImportExport;
using Microsoft.Extensions.Logging;

namespace HRManagement.WinForms.Employees;

public partial class EmployeeListControl : UserControl, IEmployeeListView
{
    private readonly EmployeeListPresenter presenter;
    private readonly IEmployeeEditorService editorService;
    private readonly IEmploymentLifecycleService lifecycleService;
    private readonly IAssignmentService assignmentService;
    private readonly IPersonnelRecordService personnelRecordService;
    private readonly IEmployeeFileService fileService;
    private readonly IEmployeeArchiveService archiveService;
    private readonly IEmployeeWorkbookService workbookService;
    private readonly IEmployeeSummaryService summaryService;
    private readonly IBackgroundExecutor backgroundExecutor;
    private readonly IPersianDateAdapter dateAdapter;
    private readonly ILogger<EmployeeEditorPresenter> editorLogger;
    private readonly ILogger<EmploymentLifecycleForm> lifecycleLogger;
    private readonly ILogger<OrganizationAssignmentsForm> organizationLogger;
    private readonly ILogger<PersonnelRecordsForm> personnelRecordsLogger;
    private readonly ILogger<FileRecordsForm> fileRecordsLogger;
    private readonly ILogger<ArchiveEmployeeDialog> archiveLogger;
    private readonly ILogger<EmployeeImportExportDialog> importExportLogger;
    private readonly CancellationTokenSource lifetime = new();
    private CancellationTokenSource? searchDebounce;
    private EmployeeEditorForm? editorForm;
    private PagedResult<EmployeeListItemDto>? currentPage;
    private EmployeeSort currentSort = new(EmployeeSortField.PersonnelNumber, SortDirection.Ascending);
    private bool disposed;

    public EmployeeListControl(
        IEmployeeSearchService searchService,
        IEmployeeEditorService editorService,
        IEmploymentLifecycleService lifecycleService,
        IAssignmentService assignmentService,
        IPersonnelRecordService personnelRecordService,
        IEmployeeFileService fileService,
        IEmployeeArchiveService archiveService,
        IEmployeeWorkbookService workbookService,
        IEmployeeSummaryService summaryService,
        IDelay delay,
        IBackgroundExecutor backgroundExecutor,
        IPersianDateAdapter dateAdapter,
        ILogger<EmployeeListPresenter> listLogger,
        ILogger<EmployeeEditorPresenter> editorLogger,
        ILogger<EmploymentLifecycleForm> lifecycleLogger,
        ILogger<OrganizationAssignmentsForm> organizationLogger,
        ILogger<PersonnelRecordsForm> personnelRecordsLogger,
        ILogger<FileRecordsForm> fileRecordsLogger,
        ILogger<ArchiveEmployeeDialog> archiveLogger,
        ILogger<EmployeeImportExportDialog> importExportLogger)
    {
        this.editorService = editorService;
        this.lifecycleService = lifecycleService;
        this.assignmentService = assignmentService;
        this.personnelRecordService = personnelRecordService;
        this.fileService = fileService;
        this.archiveService = archiveService;
        this.workbookService = workbookService;
        this.summaryService = summaryService;
        this.backgroundExecutor = backgroundExecutor;
        this.dateAdapter = dateAdapter;
        this.editorLogger = editorLogger;
        this.lifecycleLogger = lifecycleLogger;
        this.organizationLogger = organizationLogger;
        this.personnelRecordsLogger = personnelRecordsLogger;
        this.fileRecordsLogger = fileRecordsLogger;
        this.archiveLogger = archiveLogger;
        this.importExportLogger = importExportLogger;
        InitializeComponent();
        InitializeColumnVisibilityMenu();
        presenter = new EmployeeListPresenter(this, searchService, delay, listLogger, backgroundExecutor);
        WireEvents();
    }

    public string? SearchText => searchTextBox.Text;
    public int PageNumber { get; private set; } = 1;
    public int PageSize => (int)(pageSizeComboBox.SelectedItem ?? 25);
    public EmployeeSort Sort => currentSort;
    public EmployeeFilter Filter { get; private set; } = EmployeeFilter.Default;

    public Task InitializeAsync(CancellationToken cancellationToken) => presenter.RefreshAsync(cancellationToken);

    public void SetLoading(bool isLoading)
    {
        loadingLabel.Visible = isLoading;
        searchTextBox.Enabled = true;
        previousPageButton.Enabled = !isLoading && PageNumber > 1;
        nextPageButton.Enabled = !isLoading && currentPage is not null && PageNumber < currentPage.TotalPages;
    }

    public void ShowPage(PagedResult<EmployeeListItemDto> page)
    {
        currentPage = page;
        employeesGrid.DataSource = page.Items.ToList();
        emptyStateLabel.Text = "رکوردی برای نمایش وجود ندارد.";
        emptyStateLabel.Visible = page.Items.Count == 0;
        pageStatusLabel.Text = page.TotalCount == 0
            ? "بدون رکورد"
            : $"صفحه {page.PageNumber} از {Math.Max(1, page.TotalPages)} — {page.TotalCount} رکورد";
        previousPageButton.Enabled = page.PageNumber > 1;
        nextPageButton.Enabled = page.PageNumber < page.TotalPages;
        UpdateSummary();
    }

    public void ShowError(string message)
    {
        emptyStateLabel.Visible = true;
        emptyStateLabel.Text = message;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        var command = EmployeeListShortcut.Resolve(keyData);
        if (command == EmployeeListShortcutCommand.None)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        ExecuteShortcut(command);
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            lifetime.Cancel();
            lifetime.Dispose();
            searchDebounce?.Cancel();
            searchDebounce?.Dispose();
            presenter.Dispose();
            editorForm?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void WireEvents()
    {
        searchTextBox.TextChanged += async (_, _) =>
        {
            PageNumber = 1;
            searchDebounce?.Cancel();
            searchDebounce?.Dispose();
            searchDebounce = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
            try
            {
                await presenter.SearchChangedAsync(searchDebounce.Token);
            }
            catch (OperationCanceledException) when (searchDebounce.IsCancellationRequested)
            {
                return;
            }
        };
        pageSizeComboBox.SelectedIndexChanged += async (_, _) =>
        {
            PageNumber = 1;
            await presenter.RefreshAsync(lifetime.Token);
        };
        previousPageButton.Click += async (_, _) =>
        {
            PageNumber = Math.Max(1, PageNumber - 1);
            await presenter.RefreshAsync(lifetime.Token);
        };
        nextPageButton.Click += async (_, _) =>
        {
            PageNumber++;
            await presenter.RefreshAsync(lifetime.Token);
        };
        employeesGrid.SelectionChanged += (_, _) => UpdateSummary();
        columnVisibilityButton.Click += (_, _) =>
            columnVisibilityMenu.Show(columnVisibilityButton, new Point(0, columnVisibilityButton.Height));
        employeesGrid.CellDoubleClick += async (_, eventArgs) =>
        {
            if (eventArgs.RowIndex >= 0)
            {
                await OpenSelectedEmployeeAsync();
            }
        };
        employeesGrid.ColumnHeaderMouseClick += async (_, eventArgs) =>
        {
            var field = employeesGrid.Columns[eventArgs.ColumnIndex].Name switch
            {
                "firstNameColumn" => EmployeeSortField.FirstName,
                "lastNameColumn" => EmployeeSortField.LastName,
                "departmentColumn" => EmployeeSortField.Department,
                "responsibilityColumn" => EmployeeSortField.Responsibility,
                "statusColumn" => EmployeeSortField.EmploymentStatus,
                _ => EmployeeSortField.PersonnelNumber,
            };
            currentSort = new EmployeeSort(
                field,
                currentSort.Field == field && currentSort.Direction == SortDirection.Ascending
                    ? SortDirection.Descending
                    : SortDirection.Ascending);
            await presenter.RefreshAsync(lifetime.Token);
        };
        addEmployeeButton.Click += async (_, _) => await ShowEditorAsync(null);
        editEmployeeButton.Click += async (_, _) => await OpenSelectedEmployeeAsync();
        advancedFilterButton.Click += async (_, _) => await ShowAdvancedFilterAsync();
        archiveEmployeeButton.Click += async (_, _) => await ShowArchiveDialogAsync();
        importExportButton.Click += (_, _) => ShowImportExportDialog();
    }

    private void InitializeColumnVisibilityMenu()
    {
        foreach (DataGridViewColumn column in employeesGrid.Columns)
        {
            var item = new ToolStripMenuItem(column.HeaderText)
            {
                Checked = column.Visible,
                CheckOnClick = true,
                Tag = column,
            };
            item.CheckedChanged += (_, _) => ApplyColumnVisibility(item);
            columnVisibilityMenu.Items.Add(item);
        }
    }

    private void ApplyColumnVisibility(ToolStripMenuItem item)
    {
        if (item.Tag is not DataGridViewColumn column)
        {
            return;
        }

        if (!item.Checked && employeesGrid.Columns.Cast<DataGridViewColumn>().Count(candidate => candidate.Visible) <= 1)
        {
            item.Checked = true;
            return;
        }

        column.Visible = item.Checked;
    }

    private async void ExecuteShortcut(EmployeeListShortcutCommand command)
    {
        switch (command)
        {
            case EmployeeListShortcutCommand.Add:
                await ShowEditorAsync(null);
                break;
            case EmployeeListShortcutCommand.FocusSearch:
                searchTextBox.Focus();
                searchTextBox.SelectAll();
                break;
            case EmployeeListShortcutCommand.OpenSelected:
                await OpenSelectedEmployeeAsync();
                break;
        }
    }

    private async Task OpenSelectedEmployeeAsync()
    {
        if (employeesGrid.CurrentRow?.DataBoundItem is EmployeeListItemDto employee)
        {
            await ShowEditorAsync(employee.EmployeeId);
        }
    }

    private async Task ShowEditorAsync(long? employeeId)
    {
        if (editorForm is null || editorForm.IsDisposed)
        {
            editorForm = new EmployeeEditorForm(
                editorService,
                lifecycleService,
                assignmentService,
                personnelRecordService,
                fileService,
                dateAdapter,
                backgroundExecutor,
                editorLogger,
                lifecycleLogger,
                organizationLogger,
                personnelRecordsLogger,
                fileRecordsLogger);
            editorForm.EmployeeSaved += async (_, _) => await presenter.RefreshAsync(lifetime.Token);
        }

        if (!await editorForm.TryLoadAsync(employeeId, lifetime.Token))
        {
            return;
        }

        if (!editorForm.Visible)
        {
            editorForm.Show(FindForm());
        }

        editorForm.BringToFront();
        editorForm.Activate();
    }

    private void UpdateSummary()
    {
        if (employeesGrid.CurrentRow?.DataBoundItem is not EmployeeListItemDto employee)
        {
            employeeSummaryLabel.Text = "یک کارمند را انتخاب کنید.";
            editEmployeeButton.Enabled = false;
            archiveEmployeeButton.Enabled = false;
            return;
        }

        editEmployeeButton.Enabled = true;
        archiveEmployeeButton.Enabled = true;
        employeeSummaryLabel.Text =
            $"{employee.FirstName} {employee.LastName}\r\n" +
            $"شماره پرسنلی: {employee.PersonnelNumber}\r\n" +
            $"واحد: {employee.DepartmentName ?? "—"}\r\n" +
            $"مسئولیت اصلی: {employee.PrimaryResponsibility ?? "—"}";
    }

    private async Task ShowAdvancedFilterAsync()
    {
        using var dialog = new AdvancedFilterDialog(Filter);
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
        {
            return;
        }

        Filter = dialog.Filter;
        PageNumber = 1;
        await presenter.RefreshAsync(lifetime.Token);
    }

    private async Task ShowArchiveDialogAsync()
    {
        if (employeesGrid.CurrentRow?.DataBoundItem is not EmployeeListItemDto employee)
        {
            return;
        }

        using var dialog = new ArchiveEmployeeDialog(
            employee.EmployeeId,
            employee.PersonnelNumber,
            archiveService,
            backgroundExecutor,
            archiveLogger);
        dialog.ShowDialog(FindForm());
        await presenter.RefreshAsync(lifetime.Token);
    }

    private void ShowImportExportDialog()
    {
        var selectedIds = employeesGrid.SelectedRows
            .Cast<DataGridViewRow>()
            .Select(row => row.DataBoundItem)
            .OfType<EmployeeListItemDto>()
            .Select(employee => employee.EmployeeId)
            .Distinct()
            .ToArray();
        using var dialog = new EmployeeImportExportDialog(
            workbookService,
            summaryService,
            backgroundExecutor,
            Filter,
            Sort,
            SearchText,
            selectedIds,
            importExportLogger);
        dialog.ShowDialog(FindForm());
    }
}
