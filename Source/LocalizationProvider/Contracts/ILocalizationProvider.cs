namespace LocalizationProvider.Contracts;

public interface ILocalizationProvider {
    ILocalizationReader ForReadOnly(string culture);
    ILocalizationHandler ForUpdate(string culture);
}
