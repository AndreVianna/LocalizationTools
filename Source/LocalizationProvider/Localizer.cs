namespace LocalizationManager;

internal abstract class Localizer<TLocalizer>
    : ILocalizer
    where TLocalizer : Localizer<TLocalizer> {
    private readonly ILogger<TLocalizer> _logger;
    private readonly ILocalizationReader _reader;

    protected Localizer(ILocalizationReader reader, ILogger<TLocalizer> logger) {
        _reader = reader;
        _logger = logger;
    }

    protected TResult? GetResourceOrDefault<TResult>(string resourceKey, ResourceType resourceType, Func<ILocalizationReader, TResult> getLocalizedResult) {
        try {
            var result = getLocalizedResult(_reader);
            if (result is not null) {
                return result;
            }

            _logger.LogWarning("Localized {resourceType} for '{resourceKey}' not found.", resourceType, resourceKey);
            return default;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while get localized {resourceType} for '{resourceKey}'.", resourceType, resourceKey);
            return default;
        }
    }
}
