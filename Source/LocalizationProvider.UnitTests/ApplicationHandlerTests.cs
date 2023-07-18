namespace LocalizationProvider;

public class ApplicationHandlerTests {
    private readonly ILocalizationRepository _repository;
    private readonly ILogger<TextResourceHandler> _logger;
    private readonly ITextLocalizer _subject;

    public ApplicationHandlerTests() {
        var provider = Substitute.For<ILocalizationRepositoryFactory>();
        _repository = Substitute.For<ILocalizationRepository>();
        provider.CreateFor(Arg.Any<string>()).Returns(_repository);
        _logger = Substitute.For<ILogger<TextResourceHandler>>();
        var factory = new LocalizerFactory(provider, _logger.CreateFactory());
        _subject = factory.Create<TextLocalizer>("en-CA");
    }
}
