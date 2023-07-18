using Application = LocalizationProvider.PostgreSql.Schema.Application;
using DomainApplication = LocalizationProvider.Contracts.Application;

namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests {
    private static readonly Guid _defaultApplicationId = Guid.NewGuid();
    private static readonly Application _application = new() {
        Id = _defaultApplicationId,
        DefaultCulture = "en-CA",
        Name = "SomeApplication",
        AvailableCultures = new[] { "en-CA", "fr-CA" },
    };

    private void SeedApplication(Application? application = null) {
        _dbContext.Applications.Add(application ?? _application);
        _dbContext.SaveChanges();
    }

    // unit tests for the method FindApplicationById when the id exists
    [Fact]
    public void FindApplicationById_WhenApplicationExists_ReturnsApplication() {
        // Act
        var result = _repository.FindApplicationById(_defaultApplicationId);

        // Assert
        result.Should().BeOfType<DomainApplication>();
        result!.Id.Should().Be(_defaultApplicationId);
        result.Name.Should().Be(_application.Name);
        result.DefaultCulture.Should().Be(_application.DefaultCulture);
        result.AvailableCultures.Should().BeEquivalentTo(_application.AvailableCultures);
    }

    // unit tests for the method FindApplicationById when the id does not exist
    [Fact]
    public void FindApplicationById_WhenApplicationDoesNotExist_ReturnsNull() {
        // Act
        var result = _repository.FindApplicationById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    // unit tests for the method ListApplications
    [Fact]
    public void ListApplications_ReturnsAllApplications() {
        // Arrange
        var application1 = new Application {
            DefaultCulture = "en-CA",
            Name = "SomeApplication1",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };
        var application2 = new Application {
            DefaultCulture = "en-CA",
            Name = "SomeApplication2",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };
        SeedApplication(application1);
        SeedApplication(application2);

        // Act
        var result = _repository.ListApplications();

        // Assert
        var array = result.Should().BeOfType<DomainApplication[]>().Subject;
        array.Should().HaveCount(3);
        array.Should().Contain(i => i.Id == _defaultApplicationId);
        array.Should().Contain(i => i.Id == application1.Id);
        array.Should().Contain(i => i.Id == application2.Id);
    }

    // unit tests for the method AddApplication
    [Fact]
    public void AddApplication_WhenApplicationDoesNotExist_ReturnsTrue() {
        // Arrange
        var application = new DomainApplication {
            DefaultCulture = "en-CA",
            Name = "Some Application",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };

        // Act
        var result = _repository.AddApplication(application);

        // Assert
        result.Should().BeTrue();
        var entity = _dbContext.Applications.FirstOrDefault(i => i.Id == application.Id);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(application.Id);
        entity.Name.Should().Be(application.Name);
        entity.DefaultCulture.Should().Be(application.DefaultCulture);
        entity.AvailableCultures.Should().BeEquivalentTo(application.AvailableCultures);
    }

    // unit tests for the method AddApplication when id already exists
    [Fact]
    public void AddApplication_WhenApplicationExists_ReturnsFalse() {
        // Arrange
        var application = new DomainApplication {
            Id = _defaultApplicationId,
            DefaultCulture = "en-CA",
            Name = "Some Application",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };

        // Act
        var result = _repository.AddApplication(application);

        // Assert
        result.Should().BeFalse();
        var entity = _dbContext.Applications.FirstOrDefault(i => i.Id == application.Id);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(_application.Id);
        entity.Name.Should().Be(_application.Name);
        entity.DefaultCulture.Should().Be(_application.DefaultCulture);
        entity.AvailableCultures.Should().BeEquivalentTo(_application.AvailableCultures);
    }

    // unit tests for the method UpdateApplication
    [Fact]
    public void UpdateApplication_WhenApplicationExists_ReturnsTrue() {
        // Arrange
        var application = new DomainApplication {
            Id = _defaultApplicationId,
            DefaultCulture = "en-CA",
            Name = "Some Application",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };

        // Act
        var result = _repository.UpdateApplication(application);

        // Assert
        result.Should().BeTrue();
        var entity = _dbContext.Applications.FirstOrDefault(i => i.Id == application.Id);
        entity.Should().NotBeNull();
        entity!.Id.Should().Be(application.Id);
        entity.Name.Should().Be(application.Name);
        entity.DefaultCulture.Should().Be(application.DefaultCulture);
        entity.AvailableCultures.Should().BeEquivalentTo(application.AvailableCultures);
    }

    // unit tests for the method UpdateApplication when application do not exists
    [Fact]
    public void UpdateApplication_WhenApplicationDoesNotExist_ReturnsFalse() {
        // Arrange
        var application = new DomainApplication {
            Id = Guid.NewGuid(),
            DefaultCulture = "en-CA",
            Name = "Some Application",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };

        // Act
        var result = _repository.UpdateApplication(application);

        // Assert
        result.Should().BeFalse();
        var entity = _dbContext.Applications.FirstOrDefault(i => i.Id == application.Id);
        entity.Should().BeNull();
    }

    // unit tests for the method RemoveApplication
    [Fact]
    public void RemoveApplication_WhenApplicationExists_ReturnsTrue() {
        // Arrange
        var application = new Application {
            DefaultCulture = "en-CA",
            Name = "SomeApplication1",
            AvailableCultures = new[] { "en-CA", "fr-CA" },
        };
        SeedApplication(application);

        // Act
        _repository.RemoveApplication(application.Id);

        // Assert
        var entity = _dbContext.Applications.FirstOrDefault(i => i.Id == application.Id);
        entity.Should().BeNull();
    }

    // unit tests for the method RemoveApplication when application does not exist
    [Fact]
    public void RemoveApplication_WhenApplicationDoesNotExist_ReturnsFalse() {
        // Act
        var action = () => _repository.RemoveApplication(Guid.NewGuid());

        // Assert
        action.Should().NotThrow();
    }
}
