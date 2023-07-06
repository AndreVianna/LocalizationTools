using static LocalizationManager.Contracts.DateTimeFormat;

namespace LocalizationManager.PostgreSql;

public class LocalizationProviderTests {
    private readonly ServiceCollection _serviceCollection;
    private readonly LocalizationDbContext _dbContext;
    private readonly Application _application;

    public LocalizationProviderTests() {
        var applicationId = Guid.NewGuid();
        _application = new() {
            Id = applicationId,
            DefaultCulture = "en-US",
            Name = "SomeApplication",
            AvailableCultures = "en-US",
        };

        _serviceCollection = new();
        _serviceCollection.AddDbContext<LocalizationDbContext>(options => {
            options.UseInMemoryDatabase($"LocalizationDbContext_{applicationId}");
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _dbContext = serviceProvider.GetRequiredService<LocalizationDbContext>();
        _dbContext.Applications.Add(_application);
        _dbContext.SaveChanges();
    }

    private LocalizationProvider CreateProvider(Guid? applicationId = null) {
        _serviceCollection.AddSingleton<LocalizationOptions>(_ => new() { ApplicationId = applicationId ?? _application.Id });
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        return new(serviceProvider);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenApplicationDoesNotExist() {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        Action act = () => CreateProvider(invalidId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Application with id '{invalidId}' not found.");
    }

    [Fact]
    public void For_ThrowsException_WhenCultureNotAvailable() {
        // Arrange
        var provider = CreateProvider();

        // Act
        Action act = () => provider.For("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void GetDateTimeFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetDateTimeFormatKey(LongDateTimePattern);
        _dbContext.Texts
            .Add(new() {
                Id = 1234,
                Key = key,
                ApplicationId = _application.Id,
                Culture = "en-US",
                Value = "MMMM dd, yyyy",
            });
        _dbContext.SaveChanges();
        var provider = CreateProvider();

        // Act
        var result = provider.GetDateTimeFormat(key);

        // Assert
        result.Should().Be("MMMM dd, yyyy");
    }

    [Fact]
    public void GetNumberFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetNumberFormatKey(NumberFormat.CurrencyPattern, 3);
        _dbContext.Texts
            .Add(new() {
                Id = 1234,
                Key = key,
                ApplicationId = _application.Id,
                Culture = "en-US",
                Value = "c3",
            });
        _dbContext.SaveChanges();
        var provider = CreateProvider();

        // Act
        var result = provider.GetNumberFormat(key);

        // Assert
        result.Should().Be("c3");
    }

    [Fact]
    public void GetImageOrDefault_ReturnsNull_WhenImageNotFound() {
        // Arrange
        var provider = CreateProvider();

        // Act
        var result = provider.GetImageOrDefault("SomeImage");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetImageOrDefault_ReturnsImageArray_WhenImageExists() {
        // Arrange
        _dbContext.Images
            .Add(new() {
                Id = 1234,
                Key = "ImageKey",
                ApplicationId = _application.Id,
                Culture = "en-US",
                Label = "Some image.",
                Bytes = new byte[] { 1, 2, 3, 4 },
            });
        _dbContext.SaveChanges();
        var provider = CreateProvider();

        // Act
        var result = provider.GetImageOrDefault("ImageKey");

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void GetLists_ReturnsCorrectLists_WhenResourcesExist() {
        // Arrange
        _dbContext.Lists
            .Add(new() {
                Id = 1234,
                Key = "List1Key",
                ApplicationId = _application.Id,
                Culture = "en-US",
                Name = "Some list.",
                Items = new List<ListItem>
                {
                    new() {
                        ListId = 1234,
                        Index = 0,
                        Value = "Item 1",
                    },
                    new() {
                        ListId = 1234,
                        Index = 1,
                        Value = "Item 2",
                    },
                    new() {
                        ListId = 1234,
                        Index = 2,
                        Value = "Item 3",
                    },
                },
            });
        _dbContext.Lists
            .Add(new() {
                Id = 1235,
                Key = "List2Key",
                ApplicationId = _application.Id,
                Culture = "en-US",
                Name = "Some other list.",
                Items = new List<ListItem>
                {
                    new() {
                        ListId = 1235,
                        Index = 0,
                        Value = "Item 1",
                    },
                    new() {
                        ListId = 1235,
                        Index = 1,
                        Value = "Item 3",
                    },
                },
            });
        _dbContext.SaveChanges();
        var provider = CreateProvider();

        // Act
        var result = provider.GetLists();

        // Assert
        result.Should().HaveCount(2);
    }
}
