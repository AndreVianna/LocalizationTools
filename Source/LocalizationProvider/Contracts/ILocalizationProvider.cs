namespace LocalizationProvider.Contracts;

public interface ILocalizationProvider {
    ILocalizationReader AsReader(string culture);
    ILocalizationHandler AsHandler(string culture);
}
