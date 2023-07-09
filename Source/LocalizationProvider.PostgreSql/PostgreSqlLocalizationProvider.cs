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
        if (_isDisposed) return;
        _dbContext.Dispose();
        _isDisposed = true;
    }

    private void SetCulture(string culture) {
        if (!_application.AvailableCultures.Contains(culture))
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");

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
        => GetResourceOrDefault<Text, LocalizedText>(textKey);

    public LocalizedList? FindList(string listKey)
        => GetResourceOrDefault<List, LocalizedList>(listKey);

    public LocalizedImage? FindImage(string imageKey)
        => GetResourceOrDefault<Image, LocalizedImage>(imageKey);

    private TResource? GetResourceOrDefault<TEntity, TResource>(string key)
        where TEntity : Resource
        where TResource : class {
        var resourceKey = new ResourceKey(_application.Id, _culture, key);
        return _resources.Get(resourceKey, k => GetFromStorage<TEntity>(k)?.MapTo<TEntity, TResource>());
    }

    private TEntity? GetFromStorage<TEntity>(ResourceKey k) where TEntity : Resource
        => _dbContext.Set<TEntity>()
                     .AsNoTracking()
                     .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                       && r.Culture == k.Culture
                                       && r.Key == k.ResourceId);

    public void SetText(LocalizedText input) {
        AddOrUpdateText(input);
        _dbContext.SaveChanges();
    }

    public void SetList(LocalizedList input) {
        AddOrUpdateList(input);
        _dbContext.SaveChanges();
    }

    public void SetImage(LocalizedImage input) {
        AddOrUpdateImage(input);
        _dbContext.SaveChanges();
    }

    private void AddOrUpdateText(LocalizedText input) {
        var text = _dbContext
                  .Texts
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);
        if (text is null) {
            text = new Text {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Value = input.Value
            };
            _dbContext.Texts.Add(text);
        }
        else {
            text.Value = input.Value;
        }
    }

    private void AddOrUpdateList(LocalizedList input) {
        var list = _dbContext
                  .Lists
                  .Include(i => i.Items)
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Label.Key);
        if (list is null) {
            list = new List {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Label.Key,
                Label = input.Label.Value
            };
            _dbContext.Lists.Add(list);
        }
        else {
            list.Label = input.Label.Value;
        }

        list.Items.Clear();
        for (var i = 0; i < input.Items.Length; i++) {
            list.Items.Add(new ListItem {
                ListId = list.Id,
                List = list,
                Index = i,
                Key = input.Items[i].Key,
                Value = input.Items[i].Value,
            });
        }
    }

    private void AddOrUpdateImage(LocalizedImage input) {
        var image = _dbContext
                  .Images
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Label.Key);
        if (image is null) {
            image = new Image {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Label.Key,
                Label = input.Label.Value,
                Bytes = input.Bytes
            };
            _dbContext.Images.Add(image);
        }
        else {
            image.Label = input.Label.Value;
            image.Bytes = input.Bytes;
        }
    }
}
