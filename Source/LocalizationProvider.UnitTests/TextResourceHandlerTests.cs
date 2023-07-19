namespace LocalizationProvider;

public class TextResourceHandlerTests {
    private readonly ILocalizationRepository _repository;
    private readonly TextResourceHandler _subject;
    private readonly ILogger<TextResourceHandler> _logger;

    public TextResourceHandlerTests() {
        _repository = Substitute.For<ILocalizationRepository>();
        _logger = Substitute.For<ILogger<TextResourceHandler>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        _subject = new TextResourceHandler(_repository, _logger);
    }

    [Fact]
    public void Get_WithTextKey_ReturnsExpectedText() {
        // Arrange
        var text = CreateLocalizedText();
        _repository.FindTextByKey(Arg.Any<string>()).Returns(text);

        // Act
        var result = _subject.Get(text.Key);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(text);
    }

    [Fact]
    public void Get_WhenTextNotFound_ReturnsExpectedText() {
        // Arrange
        _repository.FindTextByKey(Arg.Any<string>()).Returns(default(LocalizedText));

        // Act
        var result = _subject.Get("invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Set_WithLocalizedText_SetsText() {
        // Arrange
        var text = CreateLocalizedText();

        // Act
        _subject.Set(text);

        // Assert
        _repository.Received(1).AddOrUpdateText(text);
    }

    [Fact]
    public void Set_WhenRepositoryThrowsException_ThrowsException() {
        // Arrange
        var text = CreateLocalizedText();
        _repository.When(r => r.AddOrUpdateText(Arg.Any<LocalizedText>())).Throw(new Exception());

        // Act
        var action = () => _subject.Set(text);

        // Assert
        action.Should().Throw<Exception>();
        _logger.ShouldContain(LogLevel.Error, "An error has occurred while setting a localized Text for key 'text_key'.", new(3, nameof(Extensions.LoggerExtensions.LogFailToSetResource)));
    }

    private static LocalizedText CreateLocalizedText()
        => new("text_key", "Text value");
}

