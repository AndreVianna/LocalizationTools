namespace LocalizationManager.PostgreSql;

internal sealed class LocalizationProvider : ILocalizationProvider, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    public LocalizationProvider(IServiceProvider serviceProvider) {
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

    public ILocalizationReader For(string culture) {
        if (!_application.AvailableCultures.Split(',').Contains(culture))
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");

        _culture = culture;
        return this;
    }

    public string GetDateTimeFormat(string formatKey)
        => (string)GetResourceOrDefault(formatKey, formatKey)!;

    public string GetNumberFormat(string formatKey)
        => (string)GetResourceOrDefault(formatKey, formatKey)!;

    public byte[]? GetImageOrDefault(string imageKey) {
        var key = new ResourceKey(_application.Id, _culture, imageKey);
        return (byte[]?)_resources.GetOrAdd(key, k
            => _dbContext.Images
                         .AsNoTracking()
                         .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                           && r.Culture == k.Culture
                                           && r.Key == k.ResourceId)?
                         .Bytes);
    }

    public string[] GetLists() {
        var key = new ResourceKey(_application.Id, _culture, "Lists.AllListIds");
        return (string[])_resources.GetOrAdd(key, k
            => _dbContext.Lists
                         .AsNoTracking()
                         .Where(r => r.ApplicationId == k.ApplicationId
                                  && r.Culture == k.Culture)
                         .Select(i => i.Key)
                         .ToArray())!;
    }

    public string[] GetListItems(string listKey) {
        var key = new ResourceKey(_application.Id, _culture, listKey);
        return (string[])_resources.GetOrAdd(key, k
                => _dbContext.ListItems
                             .Include(l => l.List)
                             .AsNoTracking()
                             .Where(lo => lo.List!.ApplicationId == k.ApplicationId
                                       && lo.List.Culture == k.Culture
                                       && lo.List.Key == k.ResourceId)
                             .Select(lo => lo.Value)
                             .ToArray())!;
    }

    public string GetListItem(string listKey, uint index) {
        var list = GetListItems(listKey);
        return index < list.Length
            ? list[index]
            : throw new IndexOutOfRangeException($"Item {index} of list '{listKey}' not found.");
    }

    public string GetText(string textKey)
        => (string)GetResourceOrDefault(textKey, textKey)!;

    private object? GetResourceOrDefault(string textKey, object? @default = null) {
        var key = new ResourceKey(_application.Id, _culture, textKey);
        return _resources.GetOrAdd(key, k
            => _dbContext.Texts
                   .AsNoTracking()
                   .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                        && r.Culture == k.Culture
                                        && r.Key == k.ResourceId)?
                   .Value
               ?? @default);
    }
}
