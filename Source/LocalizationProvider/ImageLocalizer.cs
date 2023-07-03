using Localization.Contracts;

namespace Localization;

public class ImageLocalizer : IImageLocalizer
{
    private readonly IResourceReader _reader;
    private readonly string _culture;

    public ImageLocalizer(IResourceReader reader, string culture)
    {
        _reader = reader;
        _culture = culture;
    }

    public Stream? this[string name]
        => _reader.For(_culture).GetImageOrDefault(name);
}
