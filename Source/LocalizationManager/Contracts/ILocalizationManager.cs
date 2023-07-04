namespace LocalizationManager.Contracts;

public interface ILocalizationManager : ILocalizationHandler {
    ILocalizationHandler For(string culture);
}