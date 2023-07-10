using static LocalizationProvider.Models.ResourceType;

namespace LocalizationProvider;

internal class ImageLocalizer
    : Localizer<ImageLocalizer>,
    IImageLocalizer {
    internal ImageLocalizer(ILocalizationReader reader, ILogger<ImageLocalizer> logger)
        : base(reader, logger) { }

    public LocalizedImage? GetLocalizedImage(string imageKey)
        => GetResourceOrDefault(imageKey, Image, rdr => rdr.FindImage(imageKey));

    public byte[]? this[string imageKey]
        => GetLocalizedImage(imageKey)?.Bytes;
}
