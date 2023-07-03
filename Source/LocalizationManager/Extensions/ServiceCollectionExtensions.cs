namespace Localization.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddLocalizationManager<TManager>(this IServiceCollection services, Guid applicationId)
        where TManager : IResourceWriter {
        return services;
    }
}
