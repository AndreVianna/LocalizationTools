namespace LocalizationManager.Contracts;

public interface ILocalizationProvider : ILocalizationReader {
    static abstract ILocalizationProvider CreateFor(Guid applicationId, IServiceProvider serviceProvider);
    ILocalizationReader For(string culture);
}