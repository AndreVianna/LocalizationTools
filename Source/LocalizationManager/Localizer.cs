namespace LocalizationManager;

internal abstract class Localizer<TLocalizer>
    : ILocalizer
    where TLocalizer : Localizer<TLocalizer>
{
    private readonly ILogger<TLocalizer> _logger;
    private readonly ILocalizationReader _reader;

    protected Localizer(ILocalizationProvider provider, string culture, ILogger<TLocalizer> logger)
    {
        _logger = logger;
        _reader = provider.For(culture);
    }

    protected TResult GetLocalizedResource<TResult>(string resourceId, LocalizerType type, TResult defaultValue, Func<ILocalizationReader, TResult> getLocalizedResult)
    {
        try
        {
            var result = getLocalizedResult(_reader);
            if (result is not null)
            {
                return result;
            }

            _logger.LogWarning("Localized {localizerType} for '{text}' not found.", type, resourceId);
            return defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while get localized {localizerType} for '{text}'.", type, resourceId);
            return defaultValue;
        }
    }
}
