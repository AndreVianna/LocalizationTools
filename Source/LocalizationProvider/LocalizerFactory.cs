namespace LocalizationProvider;

public sealed class LocalizerFactory
    : ILocalizerFactory {
    private readonly ILocalizationRepositoryFactory _repositoryFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<LocalizerKey, ILocalizer> _localizers = new();

    public LocalizerFactory(ILocalizationRepositoryFactory repositoryFactory, ILoggerFactory loggerFactory) {
        _repositoryFactory = repositoryFactory;
        _loggerFactory = loggerFactory;
    }

    public TLocalizer Create<TLocalizer>(string culture)
        where TLocalizer : class, ITypedLocalizer
        => (TLocalizer)_localizers
           .GetOrAdd(new(TLocalizer.Type, culture), k => typeof(TLocalizer).Name switch {
               nameof(TextLocalizer) => new TextLocalizer(new TextResourceHandler(_repositoryFactory.CreateFor(k.Culture), _loggerFactory.CreateLogger<TextResourceHandler>())),
               nameof(ListLocalizer) => new ListLocalizer(new ListResourceHandler(_repositoryFactory.CreateFor(k.Culture), _loggerFactory.CreateLogger<ListResourceHandler>())),
               nameof(ImageLocalizer) => new ImageLocalizer(new ImageResourceHandler(_repositoryFactory.CreateFor(k.Culture), _loggerFactory.CreateLogger<ImageResourceHandler>())),
               _ => throw new NotSupportedException($"A localizer of type '{typeof(TLocalizer).Name}' is not supported."),
           });
}
