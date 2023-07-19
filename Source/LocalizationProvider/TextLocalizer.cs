namespace LocalizationProvider;

internal sealed class TextLocalizer : ITypedLocalizer {
    private readonly TextResourceHandler _handler;

    public static ResourceType Type => ResourceType.Text;

    internal TextLocalizer(TextResourceHandler handler) {
        _handler = handler;
    }

    public string this[string templateKey, params object[] arguments] {
        get {
            var template = GetTextOrKey(templateKey);
            return arguments.Length == 0
                ? template
                : string.Format(template, arguments);
        }
    }

    public string this[DateTime dateTime, DateTimeFormat format = DateTimeFormat.DefaultDateTimePattern] {
        get {
            var key = Keys.GetDateTimeFormatKey(format);
            var pattern = GetTextOrKey(key);
            return dateTime.ToString(pattern);
        }
    }

    public string this[decimal number, int decimalPlaces = 2]
        => this[number, NumberFormat.DefaultNumberPattern, decimalPlaces];

    public string this[decimal number, NumberFormat format, int decimalPlaces = 2] {
        get {
            var key = Keys.GetNumberFormatKey(format, decimalPlaces);
            var pattern = GetTextOrKey(key);
            return number.ToString(pattern);
        }
    }

    public string this[int number, NumberFormat format = NumberFormat.DefaultNumberPattern] {
        get {
            var key = Keys.GetNumberFormatKey(format, 0);
            var pattern = GetTextOrKey(key);
            return number.ToString(pattern);
        }
    }

    private string GetTextOrKey(string key) => _handler.Get(key)?.Value ?? key;
}
