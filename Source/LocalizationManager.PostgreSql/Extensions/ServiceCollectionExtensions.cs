namespace Localization.PostgreSql.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddPostgreSqlLocalizationManager(this IServiceCollection services, Guid applicationId, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? optionsBuilder = null) {
        services.AddPostgreSqlLocalizationProvider(applicationId, connectionString, optionsBuilder);
        services.AddLocalizationManager<DatabaseLocalizationManager>(applicationId);
        return services;
    }
}
