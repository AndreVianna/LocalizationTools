using static LocalizationManager.Models.LocalizerType;

namespace LocalizationManager;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly ILocalizationProvider _provider;
    private readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(ILocalizationProvider provider)
    {
        _provider = provider;
    }

    public ITextLocalizer CreateStringLocalizer(string culture)
        => (TextLocalizer)GetOrAddLocalizer(new(Text, culture));

    public IListLocalizer CreateListLocalizer(string culture)
        => (ListLocalizer)GetOrAddLocalizer(new(List, culture));

    public IImageLocalizer CreateImageLocalizer(string culture)
        => (ImageLocalizer)GetOrAddLocalizer(new(Image, culture));

    private ILocalizer GetOrAddLocalizer(LocalizerKey key)
        => _localizers.GetOrAdd(key, k => k.Type switch {
                Text => new TextLocalizer(_provider, k.Culture),
                List => new ListLocalizer(_provider, k.Culture),
                _ => new ImageLocalizer(_provider, k.Culture),
            });
}
