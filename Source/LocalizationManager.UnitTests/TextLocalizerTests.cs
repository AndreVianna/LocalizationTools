namespace LocalizationManager;

public class TextLocalizerTests {
    private readonly ILocalizationProvider _provider;
    private readonly ILogger<TextLocalizer> _logger;
    private readonly TextLocalizer _subject;

    public TextLocalizerTests() {
        _provider = Substitute.For<ILocalizationProvider>();
        _provider.For(Arg.Any<string>()).Returns(_provider);
        _logger = Substitute.For<ILogger<TextLocalizer>>();

        _subject = new(_provider, "en-US", _logger);
    }

    [Fact]
    public void Indexer_WithTextKey_ReturnsExpectedText() {
        // Arrange
        const string textKey = "test_text_id";
        const string expectedText = "Hello, world!";
        _provider.GetText(Arg.Any<string>()).Returns(expectedText);

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(expectedText);
    }

    [Fact]
    public void Indexer_WithTextKey_WhenKeyNotFound_ReturnsTextKey() {
        // Arrange
        const string textKey = "test_text_id";
        _provider.GetText(Arg.Any<string>()).Returns(textKey);

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(textKey);
    }

    [Fact]
    public void Indexer_TemplateKey_ReturnsExpectedTemplate() {
        // Arrange
        const string templateKey = "test_template_id";
        const string expectedTemplate = @"Hello, {0}!";
        const string expectedText = "Hello, John!";
        _provider.GetText(Arg.Any<string>()).Returns(expectedTemplate);

        // Act
        var result = _subject[templateKey, "John"];

        // Assert
        result.Should().Be(expectedText);
    }

    [Fact]
    public void Indexer_DateTimeFormat_ReturnsExpectedFormattedValue() {
        // Arrange
        var dateTime = new DateTime(2021, 09, 23);
        const DateTimeFormat format = DateTimeFormat.ShortDateTimePattern;
        const string expectedPattern = "M/d/yyyy";
        var expectedFormattedValue = dateTime.ToString(expectedPattern);
        _provider.GetDateTimeFormat(Arg.Any<string>()).Returns(_ => expectedPattern);

        // Act
        var result = _subject[dateTime, format];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.3456m;
        const string expectedPattern = "c2";
        var expectedFormattedValue = number.ToString(expectedPattern);
        _provider.GetNumberFormat(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }
}
