using System.Globalization;

using Localization.Contracts;
using Localization.PostgreSql.Models;

namespace Localization.PostgreSql;

public sealed class DatabaseLocalizationProvider : IResourceReader, IDisposable {
    private readonly Application _application;
    private string _culture;

    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<Guid, Application> _applications = new();
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();
    private bool _isDisposed;

    private DatabaseLocalizationProvider(Guid applicationId, IServiceProvider serviceProvider) {
        _dbContext = serviceProvider.GetRequiredService<ResourceDbContext>();
        _application = _applications.GetOrAdd(applicationId, id
            => _dbContext.Applications.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException($"Application with id '{id}' not found."));
        _culture = _application.DefaultCulture;
    }

    public void Dispose() {
        if (_isDisposed) return;
        _dbContext.Dispose();
        _isDisposed = true;
    }

    public static IResourceReader CreateFor(Guid applicationId, IServiceProvider serviceProvider)
        => new DatabaseLocalizationProvider(applicationId, serviceProvider);

    public IResourceReader For(string culture) {
        if (!_application.AvailableCultures.Split(',').Contains(culture)) {
            throw new InvalidOperationException($"Culture '{culture}' is not available for application '{_application.Name}'.");
        }

        _culture = culture;
        return this;
    }

    public string GetDateTimeFormat(DateTimeFormat dateTimeFormat)
        => GetTextOrDefault($"{nameof(DateTimeFormat)}.{dateTimeFormat}") ??
           GetCultureDefaultDateTimeFormat(dateTimeFormat);

    public string GetNumberFormat(int decimalPlaces = 0, int integerDigits = 1)
        => GetTextOrDefault("NumberFormat.NumberPattern") ??
           GetCultureDefaultNumberFormat(decimalPlaces, integerDigits);

    public Stream? GetImageOrDefault(string imageId) {
        var key = new ResourceKey(_application.Id, _culture, imageId);
        var imageBytes = (byte[]?)_resources.GetOrAdd(key, k
            => _dbContext.Images
                         .AsNoTracking()
                         .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                           && r.Culture == k.Culture
                                           && r.Key == k.ResourceId)?
                         .Bytes);
        return imageBytes is null ? null : new MemoryStream(imageBytes);
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

    public string[]? GetListItemsOrDefault(string listId) {
        var key = new ResourceKey(_application.Id, _culture, listId);
        return !_resources.ContainsKey(key)
            ? null
            : (string[]?)_resources.GetOrAdd(key, k
                => _dbContext.ListOptions
                             .Include(l => l.List)
                             .Include(o => o.Item)
                             .AsNoTracking()
                             .Where(lo => lo.List!.ApplicationId == k.ApplicationId
                                       && lo.List.Culture == k.Culture
                                       && lo.List.Key == k.ResourceId)
                             .Select(lo => lo.Item!.Value)
                             .ToArray());
    }

    public string? GetListItemOrDefault(string listId, uint index) {
        var key = new ResourceKey(_application.Id, _culture, listId, index);
        return !_resources.ContainsKey(key)
            ? throw new InvalidOperationException($"List '{listId}' not found.")
            : (string?)_resources.GetOrAdd(key, k
               => _dbContext.ListOptions
                            .Include(l => l.List)
                            .Include(o => o.Item)
                            .AsNoTracking()
                            .FirstOrDefault(lo => lo.List!.ApplicationId == k.ApplicationId
                                               && lo.List.Culture == k.Culture
                                               && lo.List.Key == k.ResourceId
                                               && lo.Index == k.Index)?
                            .Item!.Value);
    }

    public string? GetTextOrDefault(string textId) {
        var key = new ResourceKey(_application.Id, _culture, textId);
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
}
