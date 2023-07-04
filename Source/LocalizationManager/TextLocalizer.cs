namespace LocalizationManager;

public sealed class TextLocalizer : ITextLocalizer
{
    private readonly ILocalizationReader _reader;

    internal TextLocalizer(ILocalizationProvider provider, string culture)
    {
        _reader = provider.For(culture);
    }

    public string this[string textId]
        => _reader.GetTextOrDefault(textId) ?? textId;

    public string this[string templateId, params object[] arguments]
        => string.Format(_reader.GetTextOrDefault(templateId) ?? templateId, arguments);

    public string this[DateTime dateTime, DateTimeFormat format]
        => dateTime.ToString(_reader.GetDateTimeFormat(format));

    public string this[int number, int integerDigits = 1]
        => number.ToString(_reader.GetNumberFormat(0, integerDigits));

    public string this[decimal number, int decimalPlaces = 2, int integerDigits = 1]
        => number.ToString(_reader.GetNumberFormat(decimalPlaces, integerDigits));
}
