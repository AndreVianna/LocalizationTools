using LocalizationManager.Contracts;

namespace LocalizationManager.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationManager<TManager, TManagerOptions>(this IServiceCollection services)
        where TManager : class, ILocalizationManager
        where TManagerOptions : LocalizationOptions {
        services.AddOptions<TManagerOptions>().ValidateDataAnnotations();
        services.TryAddScoped<ILocalizationManager, TManager>();
        return services;
    }

    public static IServiceCollection AddLocalizationProvider<TProvider, TProviderOptions>(this IServiceCollection services)
        where TProvider : class, ILocalizationProvider
        where TProviderOptions : LocalizationOptions {
        services.AddOptions<TProviderOptions>().ValidateDataAnnotations();
        services.TryAddSingleton<ILocalizationProvider, TProvider>();
        services.TryAddSingleton<ILocalizerFactory, LocalizerFactory>();
        return services;
    }
}
