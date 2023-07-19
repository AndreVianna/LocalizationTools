namespace LocalizationProvider;

public class ApplicationHandlerTests {
    private readonly IApplicationRepository _repository;
    private readonly ApplicationHandler _subject;

    public ApplicationHandlerTests() {
        _repository = Substitute.For<ILocalizationRepository>();
        _subject = new ApplicationHandler(_repository);
    }

    // add test for ListApplications
    [Fact]
    public void ListApplications_ReturnsExpectedApplications() {
        // Arrange
        var expectedApplications = new[] {
            CreateApplication(),
            CreateApplication(),
            CreateApplication(),
        };
        _repository.ListApplications().Returns(expectedApplications);

        // Act
        var result = _subject.ListApplications();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedApplications);
    }

    // add test for FindApplicationById
    [Fact]
    public void FindApplicationById_WithId_ReturnsExpectedApplication() {
        // Arrange
        var expectedApplication = CreateApplication();
        _repository.FindApplicationById(Arg.Any<Guid>()).Returns(expectedApplication);

        // Act
        var result = _subject.FindApplicationById(expectedApplication.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedApplication);
    }

    // add test for FindApplicationById with non-existing id
    [Fact]
    public void FindApplicationById_WithNonExistingId_ReturnsNotFound() {
        // Arrange
        var expectedApplication = CreateApplication();
        _repository.FindApplicationById(Arg.Any<Guid>()).Returns(default(Application));

        // Act
        var result = _subject.FindApplicationById(expectedApplication.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.WasNotFound.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    // add test for AddApplication with new valid application
    [Fact]
    public void AddApplication_WithValidApplication_ReturnsSuccess() {
        // Arrange
        var application = CreateApplication();
        _repository.AddApplication(application).Returns(true);

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for AddApplication with already existing id ( _repository.AddApplication returns false)
    [Fact]
    public void AddApplication_WithExistingId_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        _repository.AddApplication(application).Returns(false);

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HasConflict.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for AddApplication with invalid name
    [Fact]
    public void AddApplication_WithInvalidName_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.Name = string.Empty;

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for AddApplication with invalid DefaultCulture
    [Fact]
    public void AddApplication_WithInvalidDefaultCulture_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.DefaultCulture = string.Empty;

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    [Fact]
    public void AddApplication_WithEmptyAvailableCultures_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.AvailableCultures = Array.Empty<string>();

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    [Fact]
    public void AddApplication_WithInvalidAvailableCulture_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.AvailableCultures = new[] { string.Empty };

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    [Fact]
    public void AddApplication_WithAvailableCulturesNotContainingDefaultCulture_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.AvailableCultures = new[] { "fr-CA" };

        // Act
        var result = _subject.AddApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for UpdateApplication with a valid existing application
    [Fact]
    public void UpdateApplication_WithValidApplication_ReturnsSuccess() {
        // Arrange
        var application = CreateApplication();
        _repository.UpdateApplication(application).Returns(true);

        // Act
        var result = _subject.UpdateApplication(application);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for UpdateApplication with a non-existing application ( _repository.UpdateApplication returns false)
    [Fact]
    public void UpdateApplication_WithNonExistingApplication_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        _repository.UpdateApplication(application).Returns(false);

        // Act
        var result = _subject.UpdateApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.WasNotFound.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    // add test for UpdateApplication with invalid name
    [Fact]
    public void UpdateApplication_WithInvalidName_ReturnsFailure() {
        // Arrange
        var application = CreateApplication();
        application.Name = string.Empty;

        // Act
        var result = _subject.UpdateApplication(application);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(application);
    }

    // add test for RemoveApplication with a existing application Guid id
    [Fact]
    public void RemoveApplication_WithAnyId_ReturnsSuccess() {
        // Act
        var result = _subject.RemoveApplication(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private static Application CreateApplication()
        => new() {
            Id = Guid.NewGuid(),
            Name = "Test Application",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
            DefaultCulture = "en-CA",
        };
}
