namespace LocalizationProvider.PostgreSql.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddPostgreSqlLocalizationProvider(this IServiceCollection services,
        Action<NpgsqlDbContextOptionsBuilder>? postgreSqlOptionsBuilder = null) {
        services.AddLocalizationProvider<PostgreSqlLocalizationRepositoryFactory, PostgreSqlRepositoryOptions>();
        services.AddDbContextPool<LocalizationDbContext>((serviceProvider, optionsBuilder) => {
            var options = serviceProvider.GetRequiredService<IOptions<PostgreSqlRepositoryOptions>>().Value;
            optionsBuilder
                .UseNpgsql(options.ConnectionString, postgreSqlOptionsBuilder)
                .LogTo(Console.WriteLine);
            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (environment.IsProduction()) {
                return;
            }

            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        });

        return services;
    }
}
