using LocalizationProvider.Contracts;

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
    public void Indexer_WithListKey_WhenKeyNotFound_ReturnsEmptyList_AndLogsWarning() {
        // Arrange
        const string listKey = "list_key";
        _handler.FindList(Arg.Any<string>()).Returns(default(LocalizedList));

        // Act
        var result = _subject[listKey];

        // Assert
        result.Should().BeEmpty();
        _logger.ShouldContainSingle(LogLevel.Warning, "Localized List for 'list_key' not found.");
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
    public void Indexer_ListKey_AndLabelKey_ReturnsItemValue() {
        // Arrange
        var list = CreateLocalizedList();
        _handler.FindList(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key, Keys.ListLabel];

        // Assert
        result.Should().Be(list.Label!.Value);
    }


    [Fact]
    public void Indexer_ListKey_WhenDoNotHaveLabel_AndLabelKey_ReturnsItemValue() {
        // Arrange
        var list = CreateLocalizedListWithoutLabel();
        _handler.FindList(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject[list.Key, Keys.ListLabel];

        // Assert
        result.Should().Be(list.Key);
    }

    private static LocalizedList CreateLocalizedList() {
        var label = new LocalizedText("list_label", "List Label");
        var items = new[] {
            new LocalizedText("item_1", "Item 1"),
            new LocalizedText("item_2", null)
        };
        return new LocalizedList("list_key", label, items);
    }

    private static LocalizedList CreateLocalizedListWithoutLabel() {
        var items = new[] {
            new LocalizedText("item_1", "Item 1"),
            new LocalizedText("item_2", "Item 2")
        };
        return new LocalizedList("list_key", null, items);
    }
}
