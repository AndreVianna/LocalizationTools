using static System.Globalization.CultureInfo;

using static LocalizationManager.Contracts.DateTimeFormat;

namespace LocalizationManager.PostgreSql;

internal sealed class LocalizationProvider : ILocalizationProvider, IDisposable
{
    private readonly Application _application;
    private string _culture;

    private readonly LocalizationDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();

    public LocalizationProvider(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<LocalizationOptions>();
        _dbContext = serviceProvider.GetRequiredService<LocalizationDbContext>();
        _application = _applications.GetOrAdd(options.ApplicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException($"Application with id '{id}' not found."));
        _culture = _application.DefaultCulture;
    }

    private bool _isDisposed;
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _dbContext.Dispose();
        _isDisposed = true;
    }

    public ILocalizationReader For(string culture)
    {
        if (!_application.AvailableCultures.Split(',').Contains(culture))
        {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public string GetDateTimeFormat(DateTimeFormat format)
        => GetTextOrDefault(Keys.GetDateTimeFormatKey(format)) ??
           GetCultureDefaultDateTimeFormat(format);

    public string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1)
        => GetTextOrDefault(Keys.NumberPatternKey) ??
           GetCultureDefaultNumberFormat(decimalPlaces, integerDigits);

    public byte[]? GetImageOrDefault(string imageKey)
    {
        var key = new ResourceKey(_application.Id, _culture, imageKey);
        return (byte[]?)_resources.GetOrAdd(key, k
            => _dbContext.Images
                         .AsNoTracking()
                         .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                           && r.Culture == k.Culture
                                           && r.Key == k.ResourceId)?
                         .Bytes);
    }

    public string[] GetLists()
    {
        var key = new ResourceKey(_application.Id, _culture, "Lists.AllListIds");
        return (string[])_resources.GetOrAdd(key, k
            => _dbContext.Lists
                         .AsNoTracking()
                         .Where(r => r.ApplicationId == k.ApplicationId
                                  && r.Culture == k.Culture)
                         .Select(i => i.Key)
                         .ToArray())!;
    }

    public string[]? GetListItemsOrDefault(string listKey)
    {
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

    public string? GetListItemOrDefault(string listKey, uint index)
    {
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

    public string? GetTextOrDefault(string textKey)
    {
        var key = new ResourceKey(_application.Id, _culture, textKey);
        return (string?)_resources.GetOrAdd(key, k
                => _dbContext.Texts
                    .AsNoTracking()
                    .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                      && r.Culture == k.Culture
                                      && r.Key == k.ResourceId)?
                    .Value);
    }

    private string GetCultureDefaultDateTimeFormat(DateTimeFormat dateTimeFormat)
    {
        var dtf = GetCultureInfo(_culture).DateTimeFormat;
        return dateTimeFormat switch
        {
            LongDateTimePattern => $"{dtf.LongDatePattern} {dtf.LongTimePattern}",
            ShortDateTimePattern => $"{dtf.ShortDatePattern} {dtf.ShortTimePattern}",
            LongDatePattern => dtf.LongDatePattern,
            ShortDatePattern => dtf.ShortDatePattern,
            LongTimePattern => dtf.LongTimePattern,
            ShortTimePattern => dtf.ShortTimePattern,
            _ => dtf.SortableDateTimePattern,
        };
    }

    private static string GetCultureDefaultNumberFormat(int decimalPlaces, int integerDigits)
    {
        var whole = integerDigits <= 0 ? string.Empty : new('0', integerDigits);
        var fraction = decimalPlaces <= 0 ? string.Empty : $"{new string('0', decimalPlaces)}";
        return $"#{whole}.{fraction}";
    }
}
