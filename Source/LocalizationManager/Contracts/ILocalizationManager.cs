namespace LocalizationManager.Contracts;

public interface ILocalizationManager : ILocalizationHandler {
    static abstract ILocalizationManager CreateFor(Guid applicationId, IServiceProvider serviceProvider);
    ILocalizationHandler For(string culture);
}