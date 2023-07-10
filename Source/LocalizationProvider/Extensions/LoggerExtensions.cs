namespace LocalizationProvider.Extensions;

internal static partial class LoggerExtensions {
    [LoggerMessage(EventId = default, Level = LogLevel.Warning, Message = "Localized {resourceType} for '{resourceKey}' not found.")]
    public static partial void LogResourceNotFound(this ILogger logger, ResourceType resourceType, string resourceKey);

    [SuppressMessage("LoggingGenerator", "SYSLIB1006:Multiple logging methods cannot use the same event id within a class", Justification = "Not required.")]
    [LoggerMessage(EventId = default, Level = LogLevel.Error, Message = "An error has occurred while get localized {resourceType} for '{resourceKey}'.")]
    public static partial void LogFailToLoadResource(this ILogger logger, Exception ex, ResourceType resourceType, string resourceKey);
}
