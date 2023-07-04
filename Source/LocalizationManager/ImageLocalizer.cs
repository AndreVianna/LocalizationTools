namespace LocalizationManager;

internal class ImageLocalizer
    : Localizer<ImageLocalizer>,
    IImageLocalizer
{
    internal ImageLocalizer(ILocalizationProvider provider, string culture, ILogger<ImageLocalizer> logger)
        : base(provider, culture, logger)
    { }

    public byte[]? this[string imageId]
        => GetLocalizedResource(imageId, LocalizerType.List, Array.Empty<byte>(), rdr => rdr.GetImageOrDefault(imageId))!;
}
