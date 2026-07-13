using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRManagement.Infrastructure.Persistence;

public sealed class DateOnlyTextConverter()
    : ValueConverter<DateOnly, string>(
        date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        text => DateOnly.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture));
