namespace LocalizationProvider;

public class ImageResourceHandlerTests {
    private readonly ILocalizationRepository _repository;
    private readonly ImageResourceHandler _subject;
    private readonly ILogger<ImageResourceHandler> _logger;

    public ImageResourceHandlerTests() {
        _repository = Substitute.For<ILocalizationRepository>();
        _logger = Substitute.For<ILogger<ImageResourceHandler>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        _subject = new ImageResourceHandler(_repository, _logger);
    }

    [Fact]
    public void Get_WithImageKey_ReturnsExpectedImage() {
        // Arrange
        var image = CreateLocalizedImage();
        _repository.FindImageByKey(Arg.Any<string>()).Returns(image);

        // Act
        var result = _subject.Get(image.Key);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(image);
    }

    [Fact]
    public void Get_WhenImageNotFound_ReturnsExpectedImage() {
        // Arrange
        _repository.FindImageByKey(Arg.Any<string>()).Returns(default(LocalizedImage));

        // Act
        var result = _subject.Get("invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Set_WithLocalizedImage_SetsImage() {
        // Arrange
        var image = CreateLocalizedImage();

        // Act
        _subject.Set(image);

        // Assert
        _repository.Received(1).AddOrUpdateImage(image);
    }

    [Fact]
    public void Set_WhenRepositoryThrowsException_ThrowsException() {
        // Arrange
        var image = CreateLocalizedImage();
        _repository.When(r => r.AddOrUpdateImage(Arg.Any<LocalizedImage>())).Throw(new Exception());

        // Act
        var action = () => _subject.Set(image);

        // Assert
        action.Should().Throw<Exception>();
        _logger.ShouldContain(LogLevel.Error, "An error has occurred while setting a localized Image for key 'image_key'.", new(3, nameof(Extensions.LoggerExtensions.LogFailToSetResource)));
    }

    private static LocalizedImage CreateLocalizedImage() {
        var bytes = new byte[] { 1, 2, 3, 4, };
        return new("image_key", bytes);
    }
}
