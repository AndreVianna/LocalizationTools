namespace LocalizationProvider;

public class ListResourceHandlerTests {
    private readonly ILocalizationRepository _repository;
    private readonly ListResourceHandler _subject;
    private readonly ILogger<ListResourceHandler> _logger;

    public ListResourceHandlerTests() {
        _repository = Substitute.For<ILocalizationRepository>();
        _logger = Substitute.For<ILogger<ListResourceHandler>>();
        _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
        _subject = new ListResourceHandler(_repository, _logger);
    }

    [Fact]
    public void Get_WithListKey_ReturnsExpectedList() {
        // Arrange
        var list = CreateLocalizedList();
        _repository.FindListByKey(Arg.Any<string>()).Returns(list);

        // Act
        var result = _subject.Get(list.Key);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(list);
    }

    [Fact]
    public void Get_WhenListNotFound_ReturnsExpectedList() {
        // Arrange
        _repository.FindListByKey(Arg.Any<string>()).Returns(default(LocalizedList));

        // Act
        var result = _subject.Get("invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Set_WithLocalizedList_SetsList() {
        // Arrange
        var list = CreateLocalizedList();

        // Act
        _subject.Set(list);

        // Assert
        _repository.Received(1).AddOrUpdateList(list);
    }

    [Fact]
    public void Set_WhenRepositoryThrowsException_ThrowsException() {
        // Arrange
        var list = CreateLocalizedList();
        _repository.When(r => r.AddOrUpdateList(Arg.Any<LocalizedList>())).Throw(new Exception());

        // Act
        var action = () => _subject.Set(list);

        // Assert
        action.Should().Throw<Exception>();
        _logger.ShouldContain(LogLevel.Error, "An error has occurred while setting a localized List for key 'list_key'.", new(3, nameof(Extensions.LoggerExtensions.LogFailToSetResource)));
    }

    private static LocalizedList CreateLocalizedList() {
        var items = new LocalizedText[] {
            new("item1_Key", "Item 1"),
            new("item2_Key", "Item 2"),
            new("item3_Key", "Item 3"),
        };
        return new("list_key", items);
    }
}

