namespace HRManagement.Application.Archive;

public enum ArchiveFailureCode
{
    None = 0,
    InvalidInput,
    EmployeeNotFound,
    AlreadyArchived,
    NotArchived,
    ConfirmationMismatch,
}
