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
        if (!_application.AvailableCultures.Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public LocalizedImage? FindImage(string imageKey) => GetImageOrDefault(imageKey);

    public LocalizedList? FindList(string listKey) => GetListOrDefault(listKey);

    public LocalizedText? FindText(string textKey) => GetTextOrDefault(textKey);

    private LocalizedText? GetTextOrDefault(string textKey) {
        var key = new ResourceKey(_application.Id, _culture, textKey);
        return _resources.Get(key, k => GetTextFromStorage(k)?.MapToLocalizedText());
    }

    private Text? GetTextFromStorage(ResourceKey k)
        => _dbContext.Texts
            .AsNoTracking()
            .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                 && r.Culture == k.Culture
                                 && r.Key == k.ResourceId);

    private LocalizedList? GetListOrDefault(string listKey) {
        var key = new ResourceKey(_application.Id, _culture, listKey);
        return _resources.Get(key, k => GetListFromStorage(k)?.MapToLocalizedList());
    }

    private List? GetListFromStorage(ResourceKey k)
        => _dbContext.Lists
            .AsNoTracking()
            .FirstOrDefault(lo => lo.ApplicationId == k.ApplicationId
                                  && lo.Culture == k.Culture
                                  && lo.Key == k.ResourceId);

    private LocalizedImage? GetImageOrDefault(string imageKey) {
        var key = new ResourceKey(_application.Id, _culture, imageKey);
        return _resources.Get(key, k => GetImageFromStorage(k)?.MapToLocalizedImage());
    }

    private Image? GetImageFromStorage(ResourceKey k)
        => _dbContext.Images
            .AsNoTracking()
            .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                 && r.Culture == k.Culture
                                 && r.Key == k.ResourceId);
}
