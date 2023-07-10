namespace LocalizationProvider.Extensions;

internal static partial class LoggerExtensions {
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Localized {resourceType} for '{resourceKey}' not found.")]
    public static partial void LogResourceNotFound(this ILogger logger, ResourceType resourceType, string resourceKey);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An error has occurred while get localized {resourceType} for '{resourceKey}'.")]
    public static partial void LogFailToLoadResource(this ILogger logger, Exception ex, ResourceType resourceType, string resourceKey);
}
