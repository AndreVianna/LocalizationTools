using LocalizationManager.Contracts;

namespace LocalizationManager;

public class ImageLocalizer : IImageLocalizer
{
    private readonly IResourceReader _reader;
    private readonly string _culture;

    public ImageLocalizer(IResourceReader reader, string culture)
    {
        _reader = reader;
        _culture = culture;
    }

    public byte[]? this[string name] => _reader.For(_culture).GetImageOrDefault(name);

    public Stream? GetAsStream(string name) {
        var bytes = this[name];
        return bytes is null ? null : new MemoryStream(bytes);
    }
}
