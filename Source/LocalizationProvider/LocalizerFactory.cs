using LocalizationProvider.Contracts;
using LocalizationProvider.Models;

namespace LocalizationProvider;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly ILocalizationProvider _provider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(ILocalizationProvider provider, ILoggerFactory loggerFactory) {
        _provider = provider;
        _loggerFactory = loggerFactory;
    }

    public ITextLocalizer CreateTextLocalizer(string culture)
        => (TextLocalizer)GetOrAddLocalizer(new(Text, culture));

    public IListLocalizer CreateListLocalizer(string culture)
        => (ListLocalizer)GetOrAddLocalizer(new(List, culture));

    public IImageLocalizer CreateImageLocalizer(string culture)
        => (ImageLocalizer)GetOrAddLocalizer(new(Image, culture));

    private ILocalizer GetOrAddLocalizer(LocalizerKey key)
        => _localizers.GetOrAdd(key, k => k.Type switch {
            Text => new TextLocalizer(_provider.ForReadOnly(k.Culture), _loggerFactory.CreateLogger<TextLocalizer>()),
            List => new ListLocalizer(_provider.ForReadOnly(k.Culture), _loggerFactory.CreateLogger<ListLocalizer>()),
            _ => new ImageLocalizer(_provider.ForReadOnly(k.Culture), _loggerFactory.CreateLogger<ImageLocalizer>()),
        });
}
