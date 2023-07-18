﻿namespace LocalizationProvider;

internal abstract class Localizer<TLocalizer>
    : ILocalizer
    where TLocalizer : Localizer<TLocalizer> {
    private readonly ILogger<TLocalizer> _logger;
    private readonly IResourceReader _reader;

    protected Localizer(IResourceReader reader, ILogger<TLocalizer> logger) {
        _reader = reader;
        _logger = logger;
    }

    protected TResult? GetResourceOrDefault<TResult>(string resourceKey, ResourceType resourceType, Func<IResourceReader, TResult> getLocalizedResult) {
        try {
            var result = getLocalizedResult(_reader);
            if (result is not null) {
                return result;
            }

            _logger.LogResourceNotFound(resourceType, resourceKey);
            return default;
        }
        catch (Exception ex) {
            _logger.LogFailToLoadResource(ex, resourceType, resourceKey);
            throw;
        }
    }
}
