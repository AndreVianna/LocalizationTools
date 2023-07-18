namespace LocalizationProvider.Contracts;

public interface ILocalizationRepository {
    IResourceReader AsReader(string culture);
    IResourceRepository AsHandler(string culture);
}
