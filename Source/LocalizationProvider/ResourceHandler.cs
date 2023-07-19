namespace LocalizationProvider;

internal abstract class ResourceHandler<THandler>
    where THandler : ResourceHandler<THandler> {
    private readonly IResourceRepository _repository;
    private readonly ILogger<THandler> _logger;

    protected ResourceHandler(IResourceRepository repository, ILogger<THandler> logger) {
        _repository = repository;
        _logger = logger;
    }

    protected TResource? GetResourceOrDefault<TResource>(string resourceKey, Func<IResourceRepository, TResource?> getResource)
        where TResource : class, ILocalizedResource<TResource> {
        try {
            var result = getResource(_repository);
            if (result is not null) {
                return result;
            }

            _logger.LogResourceNotFound(ILocalizedResource<TResource>.Type, resourceKey);
            return default;
        }
        catch (Exception ex) {
            _logger.LogFailToGetResource(ex, ILocalizedResource<TResource>.Type, resourceKey);
            throw;
        }
    }

    protected void SetResource<TResource>(TResource resource, Action<IResourceRepository> setResource)
        where TResource : class, ILocalizedResource<TResource> {
        try {
            setResource(_repository);
        }
        catch (Exception ex) {
            _logger.LogFailToSetResource(ex, ILocalizedResource<TResource>.Type, resource.Key);
            throw;
        }
    }
}
