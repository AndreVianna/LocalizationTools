using System.Collections.Concurrent;

namespace LocalizationProvider;

public sealed class LocalizerFactory : ILocalizerFactory
{
    private readonly ILocalizedResourceProvider _provider;
    private static readonly ConcurrentDictionary<LocalizerType, ConcurrentDictionary<string, ILocalizer>> _localizers = new();

    public LocalizerFactory(ILocalizedResourceProvider provider)
    {
        _provider = provider;
    }

    public IStringLocalizer CreateStringLocalizer(string culture)
        => (StringLocalizer)GetOrAddLocalizer(LocalizerType.String, culture);

    public IOptionsLocalizer CreateOptionsLocalizer(string culture)
        => (OptionsLocalizer)GetOrAddLocalizer(LocalizerType.Options, culture);

    public IImageLocalizer CreateImageLocalizer(string culture)
        => (ImageLocalizer)GetOrAddLocalizer(LocalizerType.Image, culture);

    private ILocalizer GetOrAddLocalizer(LocalizerType type, string culture)
        => _localizers.GetOrAdd(type, _ => new())
            .GetOrAdd(culture, _ => type switch
            {
                LocalizerType.String => new StringLocalizer(_provider, culture),
                LocalizerType.Options => new OptionsLocalizer(_provider, culture),
                _ => new ImageLocalizer(_provider, culture),
            });
}
