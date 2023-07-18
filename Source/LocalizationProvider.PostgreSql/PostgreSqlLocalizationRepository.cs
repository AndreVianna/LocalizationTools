using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationRepository : ILocalizationRepository, IResourceRepository {
    private readonly Application _application;
    private string _culture;

    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    public PostgreSqlLocalizationRepository(LocalizationDbContext dbContext, LocalizationRepositoryOptions options) {
        _dbContext = dbContext;
        _application = _applications.GetOrAdd(options.ApplicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException($"Application with id '{id}' not found."));
        _culture = _application.DefaultCulture;
    }

    public IResourceReader AsReader(string culture) {
        SetCulture(culture);
        return this;
    }

    public IResourceRepository AsHandler(string culture) {
        SetCulture(culture);
        return this;
    }

    private void SetCulture(string culture) {
        if (!_application.AvailableCultures.Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
    }

    private TResource? GetOrDefault<TEntity, TResource>(string key)
        where TEntity : Resource
        where TResource : class, ILocalizedResource {
        var resourceKey = new ResourceKey(_application.Id, _culture, key);
        return _resources.Get(resourceKey, rk => LoadAsReadOnly<TEntity>(rk.ResourceId)?.Map<TEntity, TResource>());
    }

    private void AddOrUpdate<TEntity, TInput>(TInput input)
        where TEntity : Resource
        where TInput : class, ILocalizedResource {
        var entity = LoadForUpdate<TEntity>(input.Key);
        if (entity is null) {
            entity = input.Map<TInput, TEntity>(_application.Id, _culture, GetUpdatedText);
            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();
        }
        else {
            entity.Update(input, GetUpdatedText);
        }

        var resourceKey = new ResourceKey(_application.Id, _culture, input.Key);
        _resources[resourceKey] = input;
        _dbContext.SaveChanges();
    }

    private TEntity? LoadAsReadOnly<TEntity>(string key)
        where TEntity : Resource
        => _dbContext.Set<TEntity>()
                     .AsNoTracking()
                     .FirstOrDefault(r => r.ApplicationId == _application.Id
                                       && r.Culture == _culture
                                       && r.Key == key);

    private TEntity? LoadForUpdate<TEntity>(string key)
        where TEntity : Resource
        => _dbContext.Set<TEntity>()
                     .FirstOrDefault(r => r.ApplicationId == _application.Id
                                       && r.Culture == _culture
                                       && r.Key == key);
}
