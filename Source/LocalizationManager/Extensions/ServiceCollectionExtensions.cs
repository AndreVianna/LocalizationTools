using LocalizationManager.Contracts;

namespace LocalizationManager.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationManager<TManager>(this IServiceCollection services, Guid applicationId)
        where TManager : IResourceWriter {
        return services;
    }

    public static IServiceCollection AddLocalizationProvider<TProvider>(this IServiceCollection services, Guid applicationId)
        where TProvider : IResourceReader {
        services.AddSingleton<ILocalizerFactory>(serviceProvider => new LocalizerFactory(TProvider.CreateFor(applicationId, serviceProvider)));
        return services;
    }
}
