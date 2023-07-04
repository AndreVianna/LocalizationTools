namespace LocalizationManager;

internal sealed class TextLocalizer
    : Localizer<TextLocalizer>,
      ITextLocalizer
{
    internal TextLocalizer(ILocalizationProvider provider, string culture, ILogger<TextLocalizer> logger)
        : base(provider, culture, logger)
    { }

    public string this[string textId]
        => GetLocalizedResource(textId, LocalizerType.Text, textId, rdr => rdr.GetTextOrDefault(textId))!;

    public string this[string templateId, params object[] arguments]
        => string.Format(GetLocalizedResource(templateId, LocalizerType.Text, templateId, rdr => rdr.GetTextOrDefault(templateId))!, arguments);

    public string this[DateTime dateTime, DateTimeFormat format]
    {
        get
        {
            var pattern = GetLocalizedResource(Keys.GetDateTimeFormatKey(format), LocalizerType.Text, null, rdr => rdr.GetDateTimeFormat(format));
            return dateTime.ToString(pattern);
        }
    }

    public string this[decimal number, int decimalPlaces = 0, int integerDigits = 1]
    {
        get
        {
            var pattern = GetLocalizedResource(Keys.NumberPatternKey, LocalizerType.Text, null, rdr => rdr.GetNumberFormat(decimalPlaces, integerDigits));
            return number.ToString(pattern);
        }
    }
}
