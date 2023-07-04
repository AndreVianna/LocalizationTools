﻿namespace LocalizationManager.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationManager<TManager>(this IServiceCollection services, Guid applicationId)
        where TManager : class, ILocalizationManager {
        services.AddScoped(serviceProvider => ManagerFactory.Create<TManager>(applicationId, serviceProvider));
        return services;
    }

    public static IServiceCollection AddLocalizationProvider<TProvider>(this IServiceCollection services, Guid applicationId)
        where TProvider : class, ILocalizationProvider {
        services.AddSingleton<ILocalizerFactory>(serviceProvider => new LocalizerFactory(TProvider.CreateFor(applicationId, serviceProvider)));
        return services;
    }
}
