﻿namespace LocalizationProvider.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationProvider<TProvider>(this IServiceCollection services, string applicationId)
        where TProvider : ILocalizedResourceProvider {
        services.AddSingleton<ILocalizerFactory>(serviceProvider => new LocalizerFactory(TProvider.Create(serviceProvider, applicationId)));
        return services;
    }
}
