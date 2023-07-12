namespace LocalizationProvider;

public class ListLocalizerTests {
    private readonly ILocalizationHandler _handler;
    private readonly ILogger<ListLocalizer> _logger;
    private readonly IListLocalizer _subject;

    public ListLocalizerTests() {
        var provider = Substitute.For<ILocalizationProvider>();
        _handler = Substitute.For<ILocalizationHandler>();
        provider.ForReadOnly(Arg.Any<string>()).Returns(_handler);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<ILogger<ListLocalizer>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

        var factory = new LocalizerFactory(provider, loggerFactory);
        _subject = factory.CreateListLocalizer("en-US");
    }

    [Fact]
    public void Indexer_WithListKey_ReturnsExpectedList() {
        // Arrange
        var list = CreateLocalizedList();
        var expectedResult = list.Items.Select(i => i.Value ?? i.Key).ToArray();
        _handler.FindList(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key];

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Indexer_WithFaultyReader_Throws_AndLogsError() {
        // Arrange
        const string listKey = "list_key";
        _handler.FindList(Arg.Any<string>()).Throws(new InvalidOperationException("Some message."));

        // Act
        Action action = () => _ = _subject[listKey];

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("Some message.");
        _logger.ShouldContain(LogLevel.Error, "An error has occurred while get localized List for 'list_key'.", new(2, nameof(Extensions.LoggerExtensions.LogFailToLoadResource)));
    }

    [Fact]
    public void Indexer_WithListKey_WhenKeyNotFound_ReturnsEmptyList_AndLogsWarning() {
        // Arrange
        const string listKey = "list_key";
        _handler.FindList(Arg.Any<string>()).Returns(default(LocalizedList));

        // Act
        var result = _subject[listKey];

        // Assert
        result.Should().BeEmpty();
        _logger.ShouldContain(LogLevel.Warning, "Localized List for 'list_key' not found.", new(1, nameof(Extensions.LoggerExtensions.LogFailToLoadResource)));
    }

    [Fact]
    public void Indexer_ListKey_AndItemKey_ReturnsItemValue() {
        // Arrange
        var list = CreateLocalizedList();
        _handler.FindList(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key, list.Items[0].Key];

        // Assert
        result.Should().Be(list.Items[0].Value);
    }

    [Fact]
    public void Indexer_ListKey_AndItemKey_WhenItemHasNoValue_ReturnsItemKey() {
        // Arrange
        var list = CreateLocalizedList();
        _handler.FindList(Arg.Any<string>()).Returns(list);

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
        _handler.FindList(Arg.Any<string>()).Returns(default(LocalizedList));

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
