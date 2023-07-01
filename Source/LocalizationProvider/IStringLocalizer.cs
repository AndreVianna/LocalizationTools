namespace LocalizationProvider;

public interface IStringLocalizer : ILocalizer
{
    string this[string text] { get; }
    string this[string template, params object[] arguments] { get; }
    string this[DateTime dateTime, DateTimeType type = LocalizerOptions.DefaultDateTimeType, string? format = null] { get; }
    string this[decimal number, int decimalPlaces = 2, int integerDigits = 1] { get; }
    string this[int number, int integerDigits = 1] { get; }
}
