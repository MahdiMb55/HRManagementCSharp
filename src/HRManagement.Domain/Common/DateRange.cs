namespace HRManagement.Domain.Common;

public sealed record DateRange
{
    private DateRange(DateOnly startDate, DateOnly? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateOnly StartDate { get; }

    public DateOnly? EndDate { get; }

    public static ValidationResult<DateRange> Create(DateOnly startDate, DateOnly? endDate)
    {
        if (endDate < startDate)
        {
            return ValidationResult<DateRange>.Failure(
                "date_range.invalid_order",
                "تاریخ پایان نمی‌تواند پیش از تاریخ شروع باشد.");
        }

        return ValidationResult<DateRange>.Success(new DateRange(startDate, endDate));
    }

    public bool Overlaps(DateRange other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var thisEnd = EndDate ?? DateOnly.MaxValue;
        var otherEnd = other.EndDate ?? DateOnly.MaxValue;
        return StartDate <= otherEnd && other.StartDate <= thisEnd;
    }
}
