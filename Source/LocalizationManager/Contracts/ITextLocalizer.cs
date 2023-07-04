namespace LocalizationManager.Contracts;

public interface ITextLocalizer : ILocalizer
{
    string this[string textId] { get; }
    string this[string templateId, params object[] arguments] { get; }
    string this[DateTime dateTime, DateTimeFormat format = DateTimeFormat.SortableDateTimePattern] { get; }
    string this[decimal number, int decimalPlaces = 0, int integerDigits = 1] { get; }
}
