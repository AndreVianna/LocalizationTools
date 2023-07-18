using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal class ImageResourceHandler
    : Localizer<ImageResourceHandler>
    , IImageLocalizer
    , IImageResourceHandler {
    internal ImageResourceHandler(IResourceReader reader, ILogger<ImageResourceHandler> logger)
        : base(reader, logger) { }

    public LocalizedImage? GetLocalizedImage(string imageKey)
        => GetResourceOrDefault(imageKey, Image, rdr => rdr.FindImage(imageKey));

    public byte[]? this[string imageKey]
        => GetLocalizedImage(imageKey)?.Bytes;
}
