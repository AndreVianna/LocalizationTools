namespace LocalizationManager.Contracts;

public interface ILocalizationProvider : ILocalizationReader
{
    ILocalizationReader For(string culture);
}
