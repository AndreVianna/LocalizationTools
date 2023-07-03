namespace Localization.Contracts;

public interface ITextLocalizer : ILocalizer {
    string this[string textId] { get; }
    string this[string templateId, params object[] arguments] { get; }
    string this[DateTime dateTime, DateTimeFormat format] { get; }
    string this[decimal number, int decimalPlaces = 2, int integerDigits = 1] { get; }
    string this[int number, int integerDigits = 1] { get; }
}
