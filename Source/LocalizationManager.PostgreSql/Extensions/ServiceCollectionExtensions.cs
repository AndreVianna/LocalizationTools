namespace LocalizationManager.PostgreSql.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddPostgreSqlLocalizationManager(this IServiceCollection services, Guid applicationId, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? optionsBuilder = null) {
        services.AddDbContext<ResourceDbContext>(options => options.UseNpgsql(connectionString, optionsBuilder));
        services.AddLocalizationManager<LocalizationManager>(applicationId);
        return services;
    }

    public static IServiceCollection AddPostgreSqlLocalizationProvider(this IServiceCollection services, Guid applicationId, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? optionsBuilder = null) {
        services.AddDbContext<ResourceDbContext>(options => options.UseNpgsql(connectionString, optionsBuilder));
        services.AddLocalizationProvider<LocalizationProvider>(applicationId);
        return services;
    }
}
