namespace LocalizationManager.PostgreSql;

public sealed class LocalizationManager : ILocalizationManager, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    private LocalizationManager(Guid applicationId, IServiceProvider serviceProvider) {
        _dbContext = serviceProvider.GetRequiredService<ResourceDbContext>();
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

    public static ILocalizationManager CreateFor(Guid applicationId, IServiceProvider serviceProvider)
        => new LocalizationManager(applicationId, serviceProvider);

    public ILocalizationHandler For(string culture) {
        if (!_application.AvailableCultures.Split(',').Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public string GetDateTimeFormat(DateTimeFormat format)
        => GetTextOrDefault($"{nameof(DateTimeFormat)}.{format}") ??
           GetCultureDefaultDateTimeFormat(format);

    public string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1)
        => GetTextOrDefault("NumberFormat.NumberPattern") ??
           GetCultureDefaultNumberFormat(decimalPlaces, integerDigits);

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

    public string[]? GetListItemsOrDefault(string listKey) {
        var key = new ResourceKey(_application.Id, _culture, listKey);
        return !_resources.ContainsKey(key)
            ? null
            : (string[]?)_resources.GetOrAdd(key, k
                => _dbContext.ListItems
                             .Include(l => l.List)
                             .AsNoTracking()
                             .Where(lo => lo.List!.ApplicationId == k.ApplicationId
                                       && lo.List.Culture == k.Culture
                                       && lo.List.Key == k.ResourceId)
                             .Select(lo => lo.Value)
                             .ToArray());
    }

    public string? GetListItemOrDefault(string listKey, uint index) {
        var key = new ResourceKey(_application.Id, _culture, listKey, index);
        return !_resources.ContainsKey(key)
            ? throw new InvalidOperationException($"List '{listKey}' not found.")
            : (string?)_resources.GetOrAdd(key, k
               => _dbContext.ListItems
                            .Include(l => l.List)
                            .AsNoTracking()
                            .FirstOrDefault(lo => lo.List!.ApplicationId == k.ApplicationId
                                               && lo.List.Culture == k.Culture
                                               && lo.List.Key == k.ResourceId
                                               && lo.Index == k.Index)?
                            .Value);
    }

    public string? GetTextOrDefault(string textKey) {
        var key = new ResourceKey(_application.Id, _culture, textKey);
        return (string?)_resources.GetOrAdd(key, k
                => _dbContext.Texts
                    .AsNoTracking()
                    .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                      && r.Culture == k.Culture
                                      && r.Key == k.ResourceId)?
                    .Value);
    }

    private string GetCultureDefaultDateTimeFormat(DateTimeFormat dateTimeFormat) {
        var dtf = CultureInfo.GetCultureInfo(_culture).DateTimeFormat;
#pragma warning disable CS8524 // The switch expression is exhaustive
        return dateTimeFormat switch {
            DateTimeFormat.SortableDateTimePattern => dtf.SortableDateTimePattern,
            DateTimeFormat.LongDateTimePattern => $"{dtf.LongDatePattern} {dtf.LongTimePattern}",
            DateTimeFormat.ShortDateTimePattern => $"{dtf.ShortDatePattern} {dtf.ShortTimePattern}",
            DateTimeFormat.LongDatePattern => dtf.LongDatePattern,
            DateTimeFormat.ShortDatePattern => dtf.ShortDatePattern,
            DateTimeFormat.LongTimePattern => dtf.LongTimePattern,
            DateTimeFormat.ShortTimePattern => dtf.ShortTimePattern,
        };
#pragma warning restore CS8524
    }

    private static string GetCultureDefaultNumberFormat(int decimalPlaces, int integerDigits) {
        var whole = integerDigits <= 0 ? string.Empty : new string('0', integerDigits);
        var fraction = decimalPlaces <= 0 ? string.Empty : $"{new string('0', decimalPlaces)}";
        return $"#{whole}.{fraction}";
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
