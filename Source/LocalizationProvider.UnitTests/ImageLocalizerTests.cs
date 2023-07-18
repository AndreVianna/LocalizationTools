namespace LocalizationProvider;

public class ImageLocalizerTests {
    private readonly IResourceRepository _repository;
    private readonly ILogger<ImageResourceHandler> _logger;
    private readonly IImageLocalizer _subject;

    public ImageLocalizerTests() {
        var provider = Substitute.For<ILocalizationRepository>();
        _repository = Substitute.For<IResourceRepository>();
        provider.AsReader(Arg.Any<string>()).Returns(_repository);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _logger = Substitute.For<ILogger<ImageResourceHandler>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(_logger);

        var factory = new LocalizerFactory(provider, loggerFactory);
        _subject = factory.Create<IImageLocalizer>("en-CA");
    }

    [Fact]
    public void Indexer_WithImageKey_ReturnsExpectedImage() {
        // Arrange
        var image = CreateLocalizedImage();
        var expectedResult = image.Bytes;
        _repository.FindImage(Arg.Any<string>()).Returns(image);

        // Act
        var result = _subject[image.Key];

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Indexer_WithImageKey_WhenKeyNotFound_ReturnsEmptyImage_AndLogsWarning() {
        // Arrange
        const string imageKey = "image_key";
        _repository.FindImage(Arg.Any<string>()).Returns(default(LocalizedImage));

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
