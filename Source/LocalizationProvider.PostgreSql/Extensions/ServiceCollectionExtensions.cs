namespace LocalizationProvider.PostgreSql.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddPostgreSqlLocalizationProvider(this IServiceCollection services, Guid applicationId, string connectionString, Action<NpgsqlDbContextOptionsBuilder>? optionsBuilder = null) {
        services.AddDbContext<ResourceDbContext>(options => options.UseNpgsql(connectionString, optionsBuilder));
        services.AddLocalizationProvider<DatabaseLocalizationProvider>(applicationId);
        return services;
    }
}
