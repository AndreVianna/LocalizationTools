using LocalizationManager;

namespace LocalizationProvider;

public class TextLocalizerTests {
    private readonly ILocalizationHandler _handler;
    private readonly ILogger<TextLocalizer> _logger;
    private readonly ITextLocalizer _subject;

    public TextLocalizerTests() {
        var provider = Substitute.For<ILocalizationProvider>();
        _handler = Substitute.For<ILocalizationHandler>();
        provider.ForUpdate(Arg.Any<string>()).Returns(_handler);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<ILogger<TextLocalizer>>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

        var factory = new LocalizerFactory(provider, loggerFactory);
        _subject = factory.CreateTextLocalizer("en-US");
    }

    [Fact]
    public void Indexer_WithTextKey_ReturnsExpectedText() {
        // Arrange
        const string textKey = "test_text_id";
        var expectedText = new LocalizedText(textKey, "Hello, world!");
        _handler.FindText(Arg.Any<string>()).Returns(expectedText);

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(expectedText.Value);
    }

    [Fact]
    public void Indexer_WithTextKey_WhenKeyNotFound_ReturnsTextKey_AndLogsWarning() {
        // Arrange
        const string textKey = "test_text_id";
        _handler.FindText(Arg.Any<string>()).Returns(default(LocalizedText));

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(textKey);
        _logger.ShouldContainSingle(LogLevel.Warning, "Localized Text for 'test_text_id' not found.");
    }

    [Fact]
    public void Indexer_TemplateKey_ReturnsExpectedTemplate() {
        // Arrange
        const string templateKey = "test_template_id";
        const string expectedText = "Hello, John!";
        var expectedTemplate = new LocalizedText(templateKey, "Hello, {0}!");
        _handler.FindText(Arg.Any<string>()).Returns(expectedTemplate);

        // Act
        var result = _subject[templateKey, "John"];

        // Assert
        result.Should().Be(expectedText);
    }

    [Fact]
    public void Indexer_DateTimeFormat_ReturnsExpectedFormattedValue() {
        // Arrange
        var dateTime = new DateTime(2021, 09, 23);
        var expectedPattern = new LocalizedText(GetDateTimeFormatKey(DefaultDateTimePattern), "yyyy-MM-dd HH:mm:ss.ffffff");
        var expectedFormattedValue = dateTime.ToString(expectedPattern.Value);
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[dateTime];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_DateTimeFormat_WithFormat_ReturnsExpectedFormattedValue() {
        // Arrange
        var dateTime = new DateTime(2021, 09, 23);
        const DateTimeFormat format = ShortDateTimePattern;
        var expectedPattern = new LocalizedText(GetDateTimeFormatKey(format), "M/d/yyyy");
        var expectedFormattedValue = dateTime.ToString(expectedPattern.Value);
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[dateTime, format];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.3456m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "n2");
        const string expectedFormattedValue = "-12.35";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_WithDecimalPlaces_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.345m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "n4");
        const string expectedFormattedValue = "-12.3450";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, 4];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_WithFormat_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.345m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(PercentPattern), "p2");
        const string expectedFormattedValue = "-1,234.50%";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, PercentPattern];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_WithFormat_AndDecimalPlaces_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.345m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(CurrencyPattern), "c4");
        const string expectedFormattedValue = "-$12.3450";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, CurrencyPattern, 4];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Integer_ReturnsExpectedFormattedValue() {
        // Arrange
        const int number = -42;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "n0");
        const string expectedFormattedValue = "-42";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Integer_WithFormat_ReturnsExpectedFormattedValue() {
        // Arrange
        const int number = -42;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "e0");
        const string expectedFormattedValue = "-4e+001";
        _handler.FindText(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, ExponentialPattern];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }
}
