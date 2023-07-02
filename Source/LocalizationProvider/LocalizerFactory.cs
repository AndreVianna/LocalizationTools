using static LocalizationProvider.Models.LocalizerType;

namespace LocalizationProvider;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly ILocalizedResourceProvider _provider;
    private static readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(ILocalizedResourceProvider provider)
    {
        _provider = provider;
    }

    public ITextLocalizer CreateStringLocalizer(string culture)
        => (TextLocalizer)GetOrAddLocalizer(new(Text, culture));

    public IOptionsLocalizer CreateOptionsLocalizer(string culture)
        => (OptionsLocalizer)GetOrAddLocalizer(new(LocalizerType.Options, culture));

    public IImageLocalizer CreateImageLocalizer(string culture)
        => (ImageLocalizer)GetOrAddLocalizer(new(Image, culture));

    private ILocalizer GetOrAddLocalizer(LocalizerKey key)
        => _localizers.GetOrAdd(key, k => k.Type switch {
                Text => new TextLocalizer(_provider, k.Culture),
                LocalizerType.Options => new OptionsLocalizer(_provider, k.Culture),
                _ => new ImageLocalizer(_provider, k.Culture),
            });
}
