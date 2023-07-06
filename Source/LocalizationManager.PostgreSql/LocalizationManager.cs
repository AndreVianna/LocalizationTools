namespace LocalizationManager.PostgreSql;

public sealed class LocalizationManager : ILocalizationManager, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    private LocalizationManager(Guid applicationId, IServiceProvider serviceProvider) {
        _dbContext = serviceProvider.GetRequiredService<LocalizationDbContext>();
        _application = _applications.GetOrAdd(applicationId, id
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

    public ILocalizationHandler For(string culture) {
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

    public void SetDateTimeFormat(LocalizedDateTimeFormat format) {
        AddOrUpdateText(new LocalizedText($"{nameof(DateTimeFormat)}.{format.Key}", format.Pattern));
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
                                    && t.Key == input.Key);
        if (list is null) {
            list = new List {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Name = input.Name
            };
            _dbContext.Lists.Add(list);
        }
        else {
            list.Name = input.Name;
        }

        list.Items.Clear();
        for (var i = 0; i < input.Items.Length; i++) {
            list.Items.Add(new ListItem {
                ListId = list.Id,
                List = list,
                Index = i,
                Value = input.Items[i],
            });
        }
    }

    private void AddOrUpdateImage(LocalizedImage input) {
        var image = _dbContext
                  .Images
                  .FirstOrDefault(t => t.ApplicationId == _application.Id
                                    && t.Culture == _culture
                                    && t.Key == input.Key);
        if (image is null) {
            image = new Image {
                ApplicationId = _application.Id,
                Culture = _culture,
                Key = input.Key,
                Label = input.Label,
                Bytes = input.Bytes
            };
            _dbContext.Images.Add(image);
        }
        else {
            image.Label = input.Label;
            image.Bytes = input.Bytes;
        }
    }
}
