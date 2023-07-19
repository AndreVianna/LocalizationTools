namespace LocalizationProvider;

internal class ImageLocalizer : ITypedLocalizer {
    private readonly ImageResourceHandler _handler;

    public static ResourceType Type => ResourceType.Image;

    public ImageLocalizer(ImageResourceHandler handler) {
        _handler = handler;
    }

    public byte[]? this[string imageKey]
        => _handler.Get(imageKey)?.Bytes;
}
