namespace LocalizationProvider;

public class ImageLocalizerTests {
    private readonly ILocalizationHandler _handler;
    private readonly ILogger<ImageLocalizer> _logger;
    private readonly IImageLocalizer _subject;

    public ImageLocalizerTests() {
        var provider = Substitute.For<ILocalizationProvider>();
        _handler = Substitute.For<ILocalizationHandler>();
        provider.ForReadOnly(Arg.Any<string>()).Returns(_handler);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<ILogger<ImageLocalizer>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

        var factory = new LocalizerFactory(provider, loggerFactory);
        _subject = factory.CreateImageLocalizer("en-US");
    }

    [Fact]
    public void Indexer_WithImageKey_ReturnsExpectedImage() {
        // Arrange
        var image = CreateLocalizedImage();
        var expectedResult = image.Bytes;
        _handler.FindImage(Arg.Any<string>()).Returns(image);

        // Act
        var result = _subject[image.Key];

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Indexer_WithImageKey_WhenKeyNotFound_ReturnsEmptyImage_AndLogsWarning() {
        // Arrange
        const string imageKey = "image_key";
        _handler.FindImage(Arg.Any<string>()).Returns(default(LocalizedImage));

        // Act
        var result = _subject[imageKey];

        // Assert
        result.Should().BeNull();
        _logger.ShouldContain(LogLevel.Warning, "Localized Image for 'image_key' not found.", new(1, nameof(Extensions.LoggerExtensions.LogResourceNotFound)));
    }

    private static LocalizedImage CreateLocalizedImage() {
        var bytes = new byte[] { 1, 2, 3, 4, };
        return new("image_key", bytes);
    }
}
