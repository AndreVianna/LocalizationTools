namespace LocalizationProvider;

internal class ImageLocalizer : IImageLocalizer {
    private readonly IImageResourceHandler _handler;

    public static ResourceType Type => ResourceType.Image;

    public ImageLocalizer(IImageResourceHandler handler) {
        _handler = handler;
    }

    public byte[]? this[string imageKey]
        => _handler.Get(imageKey)?.Bytes;
}
