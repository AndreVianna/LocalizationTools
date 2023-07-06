using LocalizationManager.Contracts;

using static LocalizationManager.Contracts.DateTimeFormat;
using static LocalizationManager.Contracts.NumberFormat;
using static LocalizationManager.Models.ResourceType;

namespace LocalizationManager;

internal sealed class TextLocalizer
    : Localizer<TextLocalizer>,
      ITextLocalizer {
    internal TextLocalizer(ILocalizationProvider provider, string culture, ILogger<TextLocalizer> logger)
        : base(provider, culture, logger) { }

    public string this[string textKey]
        => GetResource(textKey, Text, rdr => rdr.GetText(textKey))!;

    public string this[string templateKey, params object[] arguments] {
        get {
            var template = GetResource(templateKey, Text, rdr => rdr.GetText(templateKey))!;
            return string.Format(template, arguments);
        }
    }

    public string this[DateTime dateTime, DateTimeFormat format = DefaultDateTimePattern] {
        get {
            var key = Keys.GetDateTimeFormatKey(format);
            var pattern = GetResource(key, Text, rdr => rdr.GetDateTimeFormat(key))!;
            return dateTime.ToString(pattern);
        }
    }

    public string this[decimal number, NumberFormat format, int decimalPlaces = 2] {
        get {
            var key = Keys.GetNumberFormatKey(format, decimalPlaces);
            var pattern = GetResource(key, Text, rdr => rdr.GetNumberFormat(key))!;
            return number.ToString(pattern);
        }
    }

    public string this[decimal number, int decimalPlaces = 2] {
        get {
            var key = Keys.GetNumberFormatKey(DefaultNumberPattern, decimalPlaces);
            var pattern = GetResource(key, Text, rdr => rdr.GetNumberFormat(key))!;
            return number.ToString(pattern);
        }
    }

    public string this[int number, NumberFormat format = DefaultNumberPattern] {
        get {
            var key = Keys.GetNumberFormatKey(DefaultNumberPattern, 0);
            var pattern = GetResource(key, Text, rdr => rdr.GetNumberFormat(key))!;
            return number.ToString(pattern);
        }
    }
}
