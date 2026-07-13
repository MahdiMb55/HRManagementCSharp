using System.Globalization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRManagement.Infrastructure.Persistence;

public sealed class NullableDateOnlyTextConverter()
    : ValueConverter<DateOnly?, string?>(
        date => date.HasValue ? date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
        text => text == null ? null : DateOnly.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture));
