namespace LocalizationProvider;

public interface ILocalizedResourceProvider
{
    static abstract ILocalizedResourceProvider Create(string applicationId);

    string? GetLocalizedTextOrDefault(string culture, string text);
    string[] GetLocalizedOptions(string culture, string category);
    string? GetLocalizedOptionOrDefault(string culture, string category, uint index);
    Stream? GetLocalizedImageOrDefault(string culture, string name);
    string GetDateTimeFormat(string culture, string toString);
    string GetNumberFormat(string culture, int integerDigits, int decimalPlaces);
}
