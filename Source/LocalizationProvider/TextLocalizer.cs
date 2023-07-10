using static LocalizationProvider.Contracts.DateTimeFormat;
using static LocalizationProvider.Contracts.NumberFormat;
using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal sealed class TextLocalizer
    : Localizer<TextLocalizer>,
      ITextLocalizer {
    internal TextLocalizer(ILocalizationReader provider, ILogger<TextLocalizer> logger)
        : base(provider, logger) { }

    public LocalizedText? GetLocalizedText(string textKey)
        => GetResourceOrDefault(textKey, Text, rdr => rdr.FindText(textKey));

    public string this[string templateKey, params object[] arguments] {
        get {
            var template = GetTextOrKey(templateKey);
            return arguments.Length == 0
                ? template
                : string.Format(template, arguments);
        }
    }

    public string this[DateTime dateTime, DateTimeFormat format = DefaultDateTimePattern] {
        get {
            var key = Keys.GetDateTimeFormatKey(format);
            var pattern = GetTextOrKey(key);
            return dateTime.ToString(pattern);
        }
    }

    public string this[decimal number, int decimalPlaces = 2]
        => this[number, DefaultNumberPattern, decimalPlaces];

    public string this[decimal number, NumberFormat format, int decimalPlaces = 2] {
        get {
            var key = Keys.GetNumberFormatKey(format, decimalPlaces);
            var pattern = GetTextOrKey(key);
            return number.ToString(pattern);
        }
    }

    public string this[int number, NumberFormat format = DefaultNumberPattern] {
        get {
            var key = Keys.GetNumberFormatKey(DefaultNumberPattern, 0);
            var pattern = GetTextOrKey(key);
            return number.ToString(pattern);
        }
    }

    private string GetTextOrKey(string key) => GetLocalizedText(key)?.Value ?? key;
}
