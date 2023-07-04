namespace LocalizationManager;

public class ImageLocalizer : IImageLocalizer
{
    private readonly ILocalizationReader _reader;

    public ImageLocalizer(ILocalizationProvider provider, string culture)
    {
        _reader = provider.For(culture);
    }

    public byte[]? this[string name] => _reader.GetImageOrDefault(name);

    public Stream? GetAsStream(string name) {
        var bytes = this[name];
        return bytes is null ? null : new MemoryStream(bytes);
    }
}
