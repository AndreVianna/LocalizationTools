using LocalizationManager.Contracts;
using LocalizationManager.Models;

namespace LocalizationManager;

internal class ImageLocalizer
    : Localizer<ImageLocalizer>,
    IImageLocalizer {
    internal ImageLocalizer(ILocalizationProvider provider, string culture, ILogger<ImageLocalizer> logger)
        : base(provider, culture, logger) { }

    public LocalizedImage? GetLocalizedImage(string imageKey)
        => GetResourceOrDefault(imageKey, Image, rdr => rdr.FindImage(imageKey));

    public byte[]? this[string imageKey]
        => GetLocalizedImage(imageKey)?.Bytes;
}
