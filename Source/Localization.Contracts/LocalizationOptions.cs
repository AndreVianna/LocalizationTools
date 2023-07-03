namespace Localization.Contracts;

public record LocalizationOptions {
    public DateTimeFormat DefaultDateTimeFormat { get; init; }
}
