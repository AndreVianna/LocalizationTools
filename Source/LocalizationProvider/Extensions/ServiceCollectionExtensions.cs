namespace LocalizationManager.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationProvider<TProvider>(this IServiceCollection services, Guid applicationId)
        where TProvider : class, ILocalizationProvider {
        services.AddSingleton<ILocalizerFactory>(serviceProvider => new LocalizerFactory(TProvider.CreateFor(applicationId, serviceProvider)));
        return services;
    }
}
