namespace LocalizationProvider.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationProvider<TRepository, TRepositoryOptions>(this IServiceCollection services)
        where TRepository : class, ILocalizationRepositoryFactory
        where TRepositoryOptions : LocalizationRepositoryOptions {
        services.AddOptions<TRepositoryOptions>().ValidateDataAnnotations();
        services.TryAddSingleton<ILocalizationRepositoryFactory, TRepository>();
        services.TryAddSingleton<ILocalizerFactory, LocalizerFactory>();
        return services;
    }
}
