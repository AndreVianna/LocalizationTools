namespace LocalizationProvider;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly ILocalizationRepository _repository;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(ILocalizationRepository repository, ILoggerFactory loggerFactory) {
        _repository = repository;
        _loggerFactory = loggerFactory;
    }

    public TLocalizer Create<TLocalizer>(string culture)
        where TLocalizer : ILocalizer {
        var key = new LocalizerKey(typeof(TLocalizer).Name, culture);
        return (TLocalizer)_localizers
           .GetOrAdd(key, k => k.LocalizerType switch {
                nameof(IListLocalizer) => new ListResourceHandler(_repository.AsReader(k.Culture), _loggerFactory.CreateLogger<ListResourceHandler>()),
                nameof(IImageLocalizer) => new ImageResourceHandler(_repository.AsReader(k.Culture), _loggerFactory.CreateLogger<ImageResourceHandler>()),
                nameof(ITextLocalizer) => new TextResourceHandler(_repository.AsReader(k.Culture), _loggerFactory.CreateLogger<TextResourceHandler>()),
                _ => throw new NotSupportedException($"Localizer of type '{k.LocalizerType}' is not supported."),
            });
    }
}
