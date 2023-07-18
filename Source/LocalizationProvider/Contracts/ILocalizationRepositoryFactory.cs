namespace LocalizationProvider.Contracts;

public interface ILocalizationRepositoryFactory {
    ILocalizationRepository CreateFor(string culture);
}
