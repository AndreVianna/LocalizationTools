namespace LocalizationProvider;

public class ListLocalizerTests {
    private readonly ILocalizationRepository _repository;
    private readonly ILogger<ListResourceHandler> _logger;
    private readonly ListLocalizer _subject;

    public ListLocalizerTests() {
        var provider = Substitute.For<ILocalizationRepositoryFactory>();
        _repository = Substitute.For<ILocalizationRepository>();
        provider.CreateFor(Arg.Any<string>()).Returns(_repository);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<ILogger<ListResourceHandler>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

        var factory = new LocalizerFactory(provider, loggerFactory);
        _subject = factory.Create<ListLocalizer>("en-CA");
    }

    [Fact]
    public void Indexer_WithListKey_ReturnsExpectedList() {
        // Arrange
        var list = CreateLocalizedList();
        var expectedResult = list.Items.Select(i => i.Value ?? i.Key).ToArray();
        _repository.FindListByKey(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key];

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Indexer_WithFaultyReader_Throws_AndLogsError() {
        // Arrange
        const string listKey = "list_key";
        _repository.FindListByKey(Arg.Any<string>()).Throws(new InvalidOperationException("Some message."));

        // Act
        Action action = () => _ = _subject[listKey];

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Some message.");
        _logger.ShouldContain(LogLevel.Error, "An error has occurred while getting a localized List with key 'list_key'.", new(2, nameof(Extensions.LoggerExtensions.LogFailToGetResource)));
    }

    [Fact]
    public void Indexer_WithListKey_WhenKeyNotFound_ReturnsEmptyList_AndLogsWarning() {
        // Arrange
        const string listKey = "list_key";
        _repository.FindListByKey(Arg.Any<string>()).Returns(default(LocalizedList));

        // Act
        var result = _subject[listKey];

        // Assert
        result.Should().BeEmpty();
        _logger.ShouldContain(LogLevel.Warning, "A localized List with key 'list_key' was not found.", new(1, nameof(Extensions.LoggerExtensions.LogResourceNotFound)));
    }

    [Fact]
    public void Indexer_ListKey_AndItemKey_ReturnsItemValue() {
        // Arrange
        var list = CreateLocalizedList();
        _repository.FindListByKey(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key, list.Items[0].Key];

        // Assert
        result.Should().Be(list.Items[0].Value);
    }

    [Fact]
    public void Indexer_ListKey_AndItemKey_WhenItemHasNoValue_ReturnsItemKey() {
        // Arrange
        var list = CreateLocalizedList();
        _repository.FindListByKey(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key, list.Items[1].Key];

        // Assert
        result.Should().Be(list.Items[1].Key);
    }

    [Fact]
    public void Indexer_ListKey_AndItemKey_WhenListNotFound_ReturnsItemKey() {
        // Arrange
        const string listKey = "list_key";
        const string itemKey = "item_key";
        _repository.FindListByKey(Arg.Any<string>()).Returns(default(LocalizedList));

        // Act
        var result = _subject[listKey, itemKey];

        // Assert
        result.Should().Be(itemKey);
    }

    private static LocalizedList CreateLocalizedList() {
        var items = new[] {
            new LocalizedText("item_1", "Item 1"),
            new LocalizedText("item_2", null)
        };

        return new("list_key", items);
    }
}
