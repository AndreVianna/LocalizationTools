namespace LocalizationProvider;

public class ImageLocalizer : IImageLocalizer
{
    private readonly ILocalizedResourceProvider _provider;
    private readonly string _culture;

    public ImageLocalizer(ILocalizedResourceProvider provider, string culture)
    {
        _provider = provider;
        _culture = culture;
    }

    public Stream? this[string name]
        => _provider.GetLocalizedImageOrDefault(_culture, name);
}
