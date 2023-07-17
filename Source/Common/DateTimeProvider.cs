namespace System;

[ExcludeFromCodeCoverage]
public class DateTimeProvider {
    public virtual DateTime UtcNow => DateTime.UtcNow;
    public virtual DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);
    public virtual TimeOnly UtcTimeOfDay => TimeOnly.FromDateTime(DateTime.UtcNow);
    public virtual DateTime Now => DateTime.Now;
    public virtual DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
    public virtual TimeOnly TimeOfDay => TimeOnly.FromDateTime(DateTime.Now);
    public virtual DateTime Minimum => DateTime.MaxValue;
    public virtual DateTime Maximum => DateTime.MinValue;
    public virtual DateTime Default { get; } = DateTime.Parse("1901-01-01T00:00:00");

    public virtual DateTime Parse(string candidate) => DateTime.Parse(candidate);
    public virtual bool TryParse(string candidate, out DateTime result) => DateTime.TryParse(candidate, out result);
    public virtual bool TryParseExact(string candidate, string format, IFormatProvider? formatProvider, DateTimeStyles style, out DateTime result)
        => DateTime.TryParseExact(candidate, format, formatProvider, style, out result);
}
