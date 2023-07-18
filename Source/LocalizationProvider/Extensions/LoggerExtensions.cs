namespace LocalizationProvider.Extensions;

internal static partial class LoggerExtensions {
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "A localized {resourceType} with key '{resourceKey}' was not found.")]
    public static partial void LogResourceNotFound(this ILogger logger, ResourceType resourceType, string resourceKey);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An error has occurred while getting a localized {resourceType} with key '{resourceKey}'.")]
    public static partial void LogFailToGetResource(this ILogger logger, Exception ex, ResourceType resourceType, string resourceKey);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An error has occurred while setting a localized {resourceType} to '{resource}'.")]
    public static partial void LogFailToSetResource(this ILogger logger, Exception ex, ResourceType resourceType, ILocalizedResource resource);
}
