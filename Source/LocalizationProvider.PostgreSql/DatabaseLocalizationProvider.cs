namespace LocalizationProvider.PostgreSql;

public sealed class DatabaseLocalizationProvider : ILocalizedResourceProvider, IDisposable
{
    private readonly string _applicationId;
    private readonly ResourceDbContext _dbContext;
    private static readonly ConcurrentDictionary<ResourceKey, object?> _resources = new();
    private bool _isDisposed;

    public DatabaseLocalizationProvider(IServiceProvider services, string applicationId)
    {
        _applicationId = applicationId;
        _dbContext = services.GetRequiredService<ResourceDbContext>();
    }

    public static ILocalizedResourceProvider Create(IServiceProvider services, string applicationId)
        => new DatabaseLocalizationProvider(services, applicationId);

    public string GetDateTimeFormat(string culture, string toString) => throw new NotImplementedException();

    public string GetNumberFormat(string culture, int integerDigits, int decimalPlaces) => throw new NotImplementedException();

    public Stream? GetLocalizedImageOrDefault(string culture, string name)
    {
        var key = new ResourceKey(_applicationId, culture, name);
        var imageBytes = (byte[]?)_resources.GetOrAdd(key, k
            => _dbContext.Images
                .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                     && r.Culture == k.Culture
                                     && r.ImageId == k.ResourceId)?
                .Bytes);
        return imageBytes is null ? null : new MemoryStream(imageBytes);
    }

    public string[] GetLocalizedOptions(string culture, string category)
    {
        var key = new ResourceKey(_applicationId, culture, category);
        return (string[])_resources.GetOrAdd(key, k
                => _dbContext.Options
                    .Where(r => r.ApplicationId == k.ApplicationId
                             && r.Culture == k.Culture
                             && r.CategoryId == k.ResourceId)
                    .OrderBy(r => r.Index)
                    .ToArray())!;
    }

    public string? GetLocalizedOptionOrDefault(string culture, string category, uint index)
    {
        var key = new ResourceKey(_applicationId, culture, category);
        return (string?)_resources.GetOrAdd(key, k
            => _dbContext.Options
                .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                  && r.Culture == k.Culture
                                  && r.CategoryId == k.ResourceId
                                  && r.Index == index)?
                .LocalizedValue);
    }

    public string? GetLocalizedTextOrDefault(string culture, string text)
    {
        var key = new ResourceKey(_applicationId, culture, text);
        return (string?)_resources.GetOrAdd(key, k
                => _dbContext.Strings
                    .FirstOrDefault(r => r.ApplicationId == k.ApplicationId
                                      && r.Culture == k.Culture
                                      && r.StringId == k.ResourceId)?
                    .LocalizedValue);
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

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(isDisposing: true);
        GC.SuppressFinalize(this);
    }

    public static ILocalizedResourceProvider Create(string applicationId) => throw new NotImplementedException();
}
