namespace LocalizationProvider;

public sealed class StringLocalizer : IStringLocalizer
{
    private readonly ILocalizedResourceProvider _provider;
    private readonly string _culture;

    internal StringLocalizer(ILocalizedResourceProvider provider, string culture)
    {
        _provider = provider;
        _culture = culture;
    }

    public string this[string text]
        => _provider.GetLocalizedTextOrDefault(_culture, text) ?? text;

    public string this[string template, params object[] arguments]
        => string.Format(_provider.GetLocalizedTextOrDefault(_culture, template) ?? template, arguments);

    public string this[DateTime dateTime, DateTimeType type = DateTimeType.PreciseDateTime, string? format = null]
        => type switch
        {
            DateTimeType.Custom => dateTime.ToString(_provider.GetDateTimeFormat(_culture, type.ToString())),
            _ => dateTime.ToString(format)
        };

    public string this[int number, int integerDigits = 1]
        => number.ToString(_provider.GetNumberFormat(_culture, integerDigits, 0));

    public string this[decimal number, int decimalPlaces = 2, int integerDigits = 1]
        => number.ToString(_provider.GetNumberFormat(_culture, integerDigits, decimalPlaces));
}
