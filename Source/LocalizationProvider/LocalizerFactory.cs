using Localization.Contracts;
using Localization.Models;

using static Localization.Models.LocalizerType;

namespace Localization;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly IResourceReader _reader;
    private static readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(IResourceReader reader)
    {
        _reader = reader;
    }

    public ITextLocalizer CreateStringLocalizer(string culture)
        => (TextLocalizer)GetOrAddLocalizer(new(Text, culture));

    public IOptionsLocalizer CreateOptionsLocalizer(string culture)
        => (ListLocalizer)GetOrAddLocalizer(new(LocalizerType.Options, culture));

    public IImageLocalizer CreateImageLocalizer(string culture)
        => (ImageLocalizer)GetOrAddLocalizer(new(Image, culture));

    private ILocalizer GetOrAddLocalizer(LocalizerKey key)
        => _localizers.GetOrAdd(key, k => k.Type switch {
                Text => new TextLocalizer(_reader, k.Culture),
                LocalizerType.Options => new ListLocalizer(_reader, k.Culture),
                _ => new ImageLocalizer(_reader, k.Culture),
            });
}
