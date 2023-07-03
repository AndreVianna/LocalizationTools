using Localization.Contracts;

namespace Localization;

public sealed class TextLocalizer : ITextLocalizer
{
    private readonly IResourceReader _reader;
    private readonly string _culture;

    internal TextLocalizer(IResourceReader reader, string culture)
    {
        _reader = reader;
        _culture = culture;
    }

    public string this[string textId]
        => _reader.For(_culture).GetTextOrDefault(textId) ?? textId;

    public string this[string templateId, params object[] arguments]
        => string.Format(_reader.For(_culture).GetTextOrDefault(templateId) ?? templateId, arguments);

    public string this[DateTime dateTime, DateTimeFormat format]
        => dateTime.ToString(_reader.For(_culture).GetDateTimeFormat(format));

    public string this[int number, int integerDigits = 1]
        => number.ToString(_reader.For(_culture).GetNumberFormat(0, integerDigits));

    public string this[decimal number, int decimalPlaces = 2, int integerDigits = 1]
        => number.ToString(_reader.For(_culture).GetNumberFormat(decimalPlaces, integerDigits));
}
