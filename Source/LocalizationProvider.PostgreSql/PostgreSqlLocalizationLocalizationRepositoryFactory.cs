using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

internal sealed class PostgreSqlLocalizationLocalizationRepositoryFactory : ILocalizationRepositoryFactory {
    private readonly Application _application;
    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();

    public PostgreSqlLocalizationLocalizationRepositoryFactory(LocalizationDbContext dbContext, LocalizationRepositoryOptions options) {
        _dbContext = dbContext;
        _application = _applications.GetOrAdd(options.ApplicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new NotSupportedException($"An application with id '{id}' was not found."));
    }

    public ILocalizationRepository CreateFor(string culture) {
        if (!_application.AvailableCultures.Contains(culture)) {
            throw new NotSupportedException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        return new PostgreSqlLocalizationRepository(_dbContext, _application, culture);
    }
}
