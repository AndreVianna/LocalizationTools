namespace LocalizationManager;

public static class ManagerFactory {
    public static ILocalizationHandler Create<TManager>(Guid applicationId, IServiceProvider serviceProvider)
        where TManager : ILocalizationManager
        => TManager.CreateFor(applicationId, serviceProvider);
}