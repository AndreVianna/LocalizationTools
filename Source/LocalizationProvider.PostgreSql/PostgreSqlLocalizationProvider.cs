using LocalizationProvider.Contracts;

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
        return _resources.Get(resourceKey, rk => GetFromStorage<TEntity>(rk.ResourceId)?.MapTo<TEntity, TResource>());
    }

    private TEntity? GetFromStorage<TEntity>(string key) where TEntity : Resource
        => _dbContext.Set<TEntity>()
                     .AsNoTracking()
                     .FirstOrDefault(r => r.ApplicationId == _application.Id
                                       && r.Culture == _culture
                                       && r.Key == key);

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
            text = input.MapTo(_application.Id, _culture);
            text.UpdateWith(input);
            _dbContext.Texts.Add(text);
        }
        else {
            text.UpdateWith(input);
        }
    }

    private void AddOrUpdateList(LocalizedList input) {
        var list = _dbContext
                  .Lists
                  .Include(i => i.Label)
                  .Include(i => i.Items)
                  .ThenInclude(i => i.Text)
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);

        if (list is null) {
            list = input.MapTo(_application.Id, _culture);
            list.UpdateWith(input, GetOrAddText);
            _dbContext.Lists.Add(list);
        }
        else {
            list.UpdateWith(input, GetOrAddText);
        }

        list.Items.Clear();
        for (var index = 0; index < input.Items.Length; index++) {
            var item = GetOrAddText(input.Items[index])!;
            list.Items.Add(item.MapTo(list, index));
        }
    }

    private void AddOrUpdateImage(LocalizedImage input) {
        var image = _dbContext
                  .Images
                  .Include(i => i.Label)
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                     && t.Culture == _culture
                                     && t.Key == input.Key);

        if (image is null) {
            image = input.MapTo(_application.Id, _culture);
            image.UpdateWith(input, GetOrAddText);
            _dbContext.Images.Add(image);
        }
        else {
            image.UpdateWith(input, GetOrAddText);
        }
    }

    private Text? GetOrAddText(LocalizedText? input) {
        if (input is null) return null;

        var text = _dbContext.Texts
                             .AsNoTracking()
                             .FirstOrDefault(r => r.ApplicationId == _application.Id
                                               && r.Culture == _culture
                                               && r.Key == input.Key
                                               && r.Value == input.Value);
        if (text is not null)
            return text;

        text = input.MapTo(_application.Id, _culture);
        text.UpdateWith(input);
        _dbContext.Texts.Add(text);
        return text;
    }
}
