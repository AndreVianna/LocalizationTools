namespace LocalizationProvider.PostgreSql;

public sealed class PostgreSqlLocalizationProvider : ILocalizationProvider, ILocalizationHandler, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    public PostgreSqlLocalizationProvider(IServiceProvider serviceProvider) {
        var options = serviceProvider.GetRequiredService<LocalizationOptions>();
        _dbContext = serviceProvider.GetRequiredService<LocalizationDbContext>();
        _application = _applications.GetOrAdd(options.ApplicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException($"Application with id '{id}' not found."));
        _culture = _application.DefaultCulture;
    }

    private bool _isDisposed;
    public void Dispose() {
        if (_isDisposed) {
            return;
        }

        _dbContext.Dispose();
        _isDisposed = true;
    }

    private void SetCulture(string culture) {
        if (!_application.AvailableCultures.Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
    }

    public ILocalizationReader ForReadOnly(string culture) {
        SetCulture(culture);
        return this;
    }

    public ILocalizationHandler ForUpdate(string culture) {
        SetCulture(culture);
        return this;
    }

    public LocalizedText? FindText(string textKey)
        => GetOrDefault<Text, LocalizedText>(textKey);

    public LocalizedList? FindList(string listKey)
        => GetOrDefault<List, LocalizedList>(listKey);

    public LocalizedImage? FindImage(string imageKey)
        => GetOrDefault<Image, LocalizedImage>(imageKey);

    public void SetText(LocalizedText input)
        => AddOrUpdate<Text, LocalizedText>(input);

    public void SetList(LocalizedList input)
        => AddOrUpdate<List, LocalizedList>(input);

    public void SetImage(LocalizedImage input)
        => AddOrUpdate<Image, LocalizedImage>(input);

    private TResource? GetOrDefault<TEntity, TResource>(string key)
        where TEntity : Resource
        where TResource : class {
        var resourceKey = new ResourceKey(_application.Id, _culture, key);
        return _resources.Get(resourceKey, rk => LoadAsReadOnly<TEntity>(rk.ResourceId)?.MapTo<TEntity, TResource>());
    }

    private void AddOrUpdate<TEntity, TInput>(TInput input)
        where TEntity : Resource
        where TInput : ILocalizedResource {
        var entity = LoadForUpdate<TEntity>(input.Key);
        if (entity is null) {
            entity = input.MapTo<TInput, TEntity>(_application.Id, _culture, GetUpdatedText);
            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();
        }
        else {
            entity.UpdateFrom(input, GetUpdatedText);
        }

        var resourceKey = new ResourceKey(_application.Id, _culture, input.Key);
        _resources[resourceKey] = input;
        _dbContext.SaveChanges();
    }

    private Text GetUpdatedText(LocalizedText input) {
        var text = LoadForUpdate<Text>(input.Key);
        if (text is not null) {
            if (text.Value == input.Value) {
                return text;
            }

            text.UpdateFrom(input, null!);
            return text;
        }

        text = input.MapTo<LocalizedText, Text>(_application.Id, _culture, null!);
        _dbContext.Texts.Add(text);
        return text;
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
