namespace LocalizationProvider.Contracts;

public interface ITextLocalizer : ILocalizer {
    LocalizedText? GetLocalizedText(string textKey);
    string this[string templateId, params object[] arguments] { get; }
    string this[DateTime dateTime, DateTimeFormat format = DateTimeFormat.DefaultDateTimePattern] { get; }
    string this[decimal number, NumberFormat format, int decimalPlaces = 2] { get; }
    string this[decimal number, int decimalPlaces = 2] { get; }
    string this[int number, NumberFormat format = NumberFormat.DefaultNumberPattern] { get; }
}
