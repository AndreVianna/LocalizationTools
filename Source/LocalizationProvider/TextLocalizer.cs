namespace LocalizationProvider;

public sealed class TextLocalizer : ITextLocalizer
{
    private readonly ILocalizedResourceProvider _provider;
    private readonly string _culture;

    internal TextLocalizer(ILocalizedResourceProvider provider, string culture)
    {
        _provider = provider;
        _culture = culture;
    }

    public string this[string text]
        => _provider.GetLocalizedTextOrDefault(_culture, text) ?? text;

    public string this[string template, params object[] arguments]
        => string.Format(_provider.GetLocalizedTextOrDefault(_culture, template) ?? template, arguments);

    public string this[DateTime dateTime, DateTimeFormat format]
        => dateTime.ToString(_provider.GetDateTimeFormat(_culture, format));

    public string this[int number, int integerDigits = 1]
        => number.ToString(_provider.GetNumberFormat(_culture, integerDigits, 0));

    public string this[decimal number, int decimalPlaces = 2, int integerDigits = 1]
        => number.ToString(_provider.GetNumberFormat(_culture, integerDigits, decimalPlaces));
}
