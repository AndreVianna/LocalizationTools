using LocalizationProvider.PostgreSql.Models;

namespace LocalizationProvider.PostgreSql;

public sealed class DatabaseLocalizationProvider : ILocalizedResourceProvider, IDisposable
{
    private readonly string _applicationId;
    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();
    private bool _isDisposed;

    public DatabaseLocalizationProvider(IServiceProvider serviceProvider, string applicationId)
    {
        _applicationId = applicationId;
        _dbContext = serviceProvider.GetRequiredService<ResourceDbContext>();
    }

    public static ILocalizedResourceProvider Create(IServiceProvider serviceProvider, string applicationId)
        => new DatabaseLocalizationProvider(serviceProvider, applicationId);
    public string GetDateTimeFormat(string culture, DateTimeFormat dateTimeFormat) => throw new NotImplementedException();

    public string GetNumberFormat(string culture, int integerDigits, int decimalPlaces) => throw new NotImplementedException();

    public Stream? GetLocalizedImageOrDefault(string culture, string name)
    {
        var key = new ResourceKey(_applicationId, culture, name);
        var imageBytes = (byte[]?)_resources.GetOrAdd(key, k
            => _dbContext.Images
                         .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                           && r.Culture == k.Culture
                                           && r.ResourceId == k.ResourceId)?
                         .Bytes);
        return imageBytes is null ? null : new MemoryStream(imageBytes);
    }

    public string[] GetLocalizedList(string culture, string listId)
    {
        var key = new ResourceKey(_applicationId, culture, listId);
        return (string[]?)_resources.GetOrAdd(key, k
            => _dbContext.ListOptions
                         .Include(l => l.List)
                         .Include(o => o.Option)
                         .Where(lo => lo.List.ApplicationId == k.ApplicationId
                                   && lo.List.Culture == k.Culture
                                   && lo.List.ResourceId == k.ResourceId)
                         .Select(lo => lo.Option.Value)
                         .ToArray()) ?? Array.Empty<string>();
    }

    public string? GetLocalizedOptionOrDefault(string culture, string listId, uint index)
    {
        var key = new ResourceKey(_applicationId, culture, listId, index);
        return (string?)_resources.GetOrAdd(key, k
            => _dbContext.ListOptions
                         .Include(l => l.List)
                         .Include(o => o.Option)
                         .FirstOrDefault(lo => lo.List.ApplicationId == k.ApplicationId
                                       && lo.List.Culture == k.Culture
                                       && lo.List.ResourceId == k.ResourceId
                                       && lo.Index == k.Index)?
                         .Option.Value);
    }

    public string? GetLocalizedTextOrDefault(string culture, string text)
    {
        var key = new ResourceKey(_applicationId, culture, text);
        return (string?)_resources.GetOrAdd(key, k
                => _dbContext.Texts
                    .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                      && r.Culture == k.Culture
                                      && r.ResourceId == k.ResourceId)?
                    .Value);
    }

    private void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            _dbContext.Dispose();
        }

        _isDisposed = true;
    }

    public void Dispose() => Dispose(isDisposing: true);

    public static ILocalizedResourceProvider Create(string applicationId) => throw new NotImplementedException();
}
