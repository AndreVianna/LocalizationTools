using LocalizationManager.Contracts;

namespace LocalizationManager;

internal class ImageLocalizer
    : Localizer<ImageLocalizer>,
    IImageLocalizer {
    internal ImageLocalizer(ILocalizationProvider provider, string culture, ILogger<ImageLocalizer> logger)
        : base(provider, culture, logger) { }

    public byte[]? this[string imageKey]
        => GetResource(imageKey, ResourceType.List, rdr => rdr.GetImageOrDefault(imageKey)) ?? Array.Empty<byte>();
}
