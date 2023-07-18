namespace LocalizationProvider;

public class TextResourceHandlerTests {
    private readonly ILocalizationRepository _repository;
    private readonly ILogger<TextResourceHandler> _logger;
    private readonly ITextLocalizer _subject;

    public TextResourceHandlerTests() {
        var provider = Substitute.For<ILocalizationRepositoryFactory>();
        _repository = Substitute.For<ILocalizationRepository>();
        provider.CreateFor(Arg.Any<string>()).Returns(_repository);
        _logger = Substitute.For<ILogger<TextResourceHandler>>();
        var factory = new LocalizerFactory(provider, _logger.CreateFactory());
        _subject = factory.Create<TextLocalizer>("en-CA");
    }

    [Fact]
    public void Indexer_WithTextKey_ReturnsExpectedText() {
        // Arrange
        const string textKey = "text_key";
        var expectedText = new LocalizedText(textKey, "Hello, world!");
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedText);

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(expectedText.Value);
    }

    [Fact]
    public void Indexer_WithTextKey_WhenKeyNotFound_ReturnsTextKey_AndLogsWarning() {
        // Arrange
        const string textKey = "text_key";
        _repository.FindTextByKey(Arg.Any<string>()).Returns(default(LocalizedText));

        // Act
        var result = _subject[textKey];

        // Assert
        result.Should().Be(textKey);
        _logger.ShouldContain(LogLevel.Warning, "A localized Text with key 'text_key' was not found.", new(1, nameof(Extensions.LoggerExtensions.LogResourceNotFound)));
    }

    [Fact]
    public void Indexer_TemplateKey_ReturnsExpectedTemplate() {
        // Arrange
        const string templateKey = "template_key";
        const string expectedText = "Hello, John!";
        var expectedTemplate = new LocalizedText(templateKey, "Hello, {0}!");
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedTemplate);

        // Act
        var result = _subject[templateKey, "John"];

        // Assert
        result.Should().Be(expectedText);
    }

    [Fact]
    public void Indexer_DateTimeFormat_WithoutFormat_ReturnsExpectedFormattedValue_WithDefaultDateTimePattern() {
        // Arrange
        var dateTime = new DateTime(2021, 09, 23);
        var expectedFormattedValue = dateTime.ToString(GetDateTimeFormatKey(DefaultDateTimePattern));
        _repository.FindTextByKey(Arg.Any<string>()).Returns(default(LocalizedText));

        // Act
        var result = _subject[dateTime];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Theory]
    [InlineData(DefaultDateTimePattern, true)]
    [InlineData(LongDateTimePattern, true)]
    [InlineData(ShortDateTimePattern, true)]
    [InlineData(LongDatePattern, true)]
    [InlineData(ShortDatePattern, true)]
    [InlineData(LongTimePattern, true)]
    [InlineData(ShortTimePattern, true)]
    [InlineData(DefaultDateTimePattern, false)]
    [InlineData(LongDateTimePattern, false)]
    [InlineData(ShortDateTimePattern, false)]
    [InlineData(LongDatePattern, false)]
    [InlineData(ShortDatePattern, false)]
    [InlineData(LongTimePattern, false)]
    [InlineData(ShortTimePattern, false)]
    public void Indexer_DateTimeFormat_ReturnsExpectedFormattedValue(DateTimeFormat format, bool useCustomFormat) {
        // Arrange
        var dateTime = new DateTime(2021, 09, 23);
        var expectedPattern = new LocalizedText(GetDateTimeFormatKey(format), useCustomFormat ? "M/d/yyyy" : GetDateTimeFormatKey(format));
        var expectedFormattedValue = dateTime.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[dateTime];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Decimal_ReturnsExpectedFormattedValue() {
        // Arrange
        const decimal number = -12.3456m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "n2");
        const string expectedFormattedValue = "-12.35";
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

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
        var expectedFormattedValue = number.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, 4];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Theory]
    [InlineData(DefaultNumberPattern, true)]
    [InlineData(CurrencyPattern, true)]
    [InlineData(PercentPattern, true)]
    [InlineData(ExponentialPattern, true)]
    [InlineData(DefaultNumberPattern, false)]
    [InlineData(CurrencyPattern, false)]
    [InlineData(PercentPattern, false)]
    [InlineData(ExponentialPattern, false)]
    public void Indexer_Decimal_WithFormat_ReturnsExpectedFormattedValue(NumberFormat format, bool useCustomFormat) {
        // Arrange
        const decimal number = -12.345m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(format), useCustomFormat ? "p2" : GetNumberFormatKey(format));
        var expectedFormattedValue = number.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, format];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Theory]
    [InlineData(DefaultNumberPattern, 0)]
    [InlineData(CurrencyPattern, 0)]
    [InlineData(PercentPattern, 0)]
    [InlineData(ExponentialPattern, 0)]
    [InlineData(DefaultNumberPattern, 1)]
    [InlineData(CurrencyPattern, 1)]
    [InlineData(PercentPattern, 1)]
    [InlineData(ExponentialPattern, 1)]
    [InlineData(DefaultNumberPattern, 2)]
    [InlineData(CurrencyPattern, 2)]
    [InlineData(PercentPattern, 2)]
    [InlineData(ExponentialPattern, 2)]
    [InlineData(DefaultNumberPattern, 3)]
    [InlineData(CurrencyPattern, 3)]
    [InlineData(PercentPattern, 3)]
    [InlineData(ExponentialPattern, 3)]
    [InlineData(DefaultNumberPattern, 4)]
    [InlineData(CurrencyPattern, 4)]
    [InlineData(PercentPattern, 4)]
    [InlineData(ExponentialPattern, 4)]
    [InlineData(DefaultNumberPattern, 5)]
    [InlineData(CurrencyPattern, 5)]
    [InlineData(PercentPattern, 5)]
    [InlineData(ExponentialPattern, 5)]
    [InlineData(DefaultNumberPattern, 6)]
    [InlineData(CurrencyPattern, 6)]
    [InlineData(PercentPattern, 6)]
    [InlineData(ExponentialPattern, 6)]
    public void Indexer_Decimal_WithFormat_AndDecimalPlaces_ReturnsExpectedFormattedValue(NumberFormat format, int decimalPlaces) {
        // Arrange
        const decimal number = -12.345m;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(format), GetNumberFormatKey(CurrencyPattern));
        var expectedFormattedValue = number.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, format, decimalPlaces];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Fact]
    public void Indexer_Integer_ReturnsExpectedFormattedValue() {
        // Arrange
        const int number = -42;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(DefaultNumberPattern), "n0");
        var expectedFormattedValue = number.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }

    [Theory]
    [InlineData(DefaultNumberPattern, true)]
    [InlineData(CurrencyPattern, true)]
    [InlineData(PercentPattern, true)]
    [InlineData(ExponentialPattern, true)]
    [InlineData(DefaultNumberPattern, false)]
    [InlineData(CurrencyPattern, false)]
    [InlineData(PercentPattern, false)]
    [InlineData(ExponentialPattern, false)]
    public void Indexer_Integer_WithFormat_ReturnsExpectedFormattedValue(NumberFormat format, bool useCustomFormat) {
        // Arrange
        const int number = -42;
        var expectedPattern = new LocalizedText(GetNumberFormatKey(format), useCustomFormat ? "n0" : GetNumberFormatKey(format));
        var expectedFormattedValue = number.ToString(expectedPattern.Value);
        _repository.FindTextByKey(Arg.Any<string>()).Returns(expectedPattern);

        // Act
        var result = _subject[number, format];

        // Assert
        result.Should().Be(expectedFormattedValue);
    }
}
