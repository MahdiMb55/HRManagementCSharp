using HRManagement.Application.Abstractions;
using HRManagement.Application.Employees;
using HRManagement.Domain.Enums;

namespace HRManagement.Application.Tests.Employees;

public sealed class EmployeeEditorServiceTests
{
    private static readonly DateOnly FixedToday = new(2026, 7, 14);
    private static readonly DateTime NowUtc = new(2026, 7, 14, 8, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task SaveAsync_CreatesValidatedEmployee()
    {
        var repository = new FakeEmployeeWriteRepository();
        var service = new EmployeeEditorService(repository, new FixedClock());
        var request = ValidRequest();

        var result = await service.SaveAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.EmployeeId);
        Assert.Equal("0013548581", repository.LastSave!.NationalCode);
        Assert.Equal("0012", repository.LastSave.PersonnelNumber);
        Assert.Equal("09121234567", repository.LastSave.MobileNumber);
        Assert.Equal("employee.created", repository.LastSave.AuditAction);
    }

    [Fact]
    public async Task SaveAsync_RejectsInvalidNationalCodeBeforeRepositoryWrite()
    {
        var repository = new FakeEmployeeWriteRepository();
        var service = new EmployeeEditorService(repository, new FixedClock());

        var result = await service.SaveAsync(ValidRequest() with { NationalCode = "123" }, CancellationToken.None);

        Assert.Equal(EmployeeSaveFailureCode.InvalidNationalCode, result.FailureCode);
        Assert.Null(repository.LastSave);
    }

    [Fact]
    public async Task SaveAsync_RejectsDuplicateIdentifiers()
    {
        var repository = new FakeEmployeeWriteRepository
        {
            NationalCodeExists = true,
            PersonnelNumberExists = true,
        };
        var service = new EmployeeEditorService(repository, new FixedClock());

        var nationalCodeResult = await service.SaveAsync(ValidRequest(), CancellationToken.None);
        repository.NationalCodeExists = false;
        var personnelNumberResult = await service.SaveAsync(ValidRequest(), CancellationToken.None);

        Assert.Equal(EmployeeSaveFailureCode.DuplicateNationalCode, nationalCodeResult.FailureCode);
        Assert.Equal(EmployeeSaveFailureCode.DuplicatePersonnelNumber, personnelNumberResult.FailureCode);
    }

    [Fact]
    public async Task SaveAsync_RejectsFutureBirthDate()
    {
        var service = new EmployeeEditorService(new FakeEmployeeWriteRepository(), new FixedClock());

        var result = await service.SaveAsync(
            ValidRequest() with { BirthDate = FixedToday.AddDays(1) },
            CancellationToken.None);

        Assert.Equal(EmployeeSaveFailureCode.FutureBirthDate, result.FailureCode);
    }

    [Fact]
    public async Task SaveAsync_RequiresDedicatedWorkflowToChangePersonnelNumber()
    {
        var repository = new FakeEmployeeWriteRepository
        {
            Existing = new EmployeeEditDto(9, "علی", "رضایی", "0099", "0013548581", null, Gender.Male, null, null),
        };
        var service = new EmployeeEditorService(repository, new FixedClock());

        var result = await service.SaveAsync(ValidRequest() with { EmployeeId = 9 }, CancellationToken.None);

        Assert.Equal(EmployeeSaveFailureCode.PersonnelNumberChangeRequiresReason, result.FailureCode);
        Assert.Null(repository.LastSave);
    }

    private static SaveEmployeeRequest ValidRequest() =>
        new(null, "علی", "رضایی", "0012", "0013548581", "حسن", Gender.Male, new DateOnly(1990, 1, 1), "۰۹۱۲۱۲۳۴۵۶۷");

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow => NowUtc;
        public DateOnly Today => FixedToday;
    }

    private sealed class FakeEmployeeWriteRepository : IEmployeeWriteRepository
    {
        public bool NationalCodeExists { get; set; }
        public bool PersonnelNumberExists { get; set; }
        public EmployeeEditDto? Existing { get; set; }
        public ValidatedEmployeeSave? LastSave { get; private set; }

        public Task<EmployeeEditDto?> FindForEditAsync(long employeeId, CancellationToken cancellationToken) =>
            Task.FromResult(Existing);

        public Task<bool> NationalCodeExistsAsync(string nationalCode, long? excludingEmployeeId, CancellationToken cancellationToken) =>
            Task.FromResult(NationalCodeExists);

        public Task<bool> PersonnelNumberExistsAsync(string personnelNumber, long? excludingEmployeeId, CancellationToken cancellationToken) =>
            Task.FromResult(PersonnelNumberExists);

        public Task<long> SaveAsync(ValidatedEmployeeSave employee, CancellationToken cancellationToken)
        {
            LastSave = employee;
            return Task.FromResult(42L);
        }
    }
}
