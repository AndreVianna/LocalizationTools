using Microsoft.Extensions.DependencyInjection;

namespace LocalizationProvider;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLocalizationProvider<TProvider>(this IServiceCollection services, string applicationId)
        where TProvider : ILocalizedResourceProvider
    {
        services.AddSingleton<ILocalizerFactory>(new LocalizerFactory(TProvider.Create(applicationId)));
        return services;
    }
}
