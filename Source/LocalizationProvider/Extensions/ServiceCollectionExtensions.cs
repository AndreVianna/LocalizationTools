namespace LocalizationManager.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationHandler<THandler, THandlerOptions>(this IServiceCollection services)
        where THandler : class, ILocalizationProvider
        where THandlerOptions : LocalizationOptions {
        services.AddOptions<THandlerOptions>().ValidateDataAnnotations();
        services.TryAddScoped<ILocalizationProvider, THandler>();
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
