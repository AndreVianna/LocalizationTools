using LocalizationManager.Contracts;
using LocalizationManager.Models;
using LocalizationManager.PostgreSql.Schema;

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
            AvailableCultures = new[] { "en-CA", "fr-CA" },
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
    public void FindText_DateTimeProviderFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetDateTimeFormatKey(DateTimeFormat.LongDateTimePattern);
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
        var result = provider.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("dddd, dd MMMM yyyy HH:mm:ss");
        subject.Value.Should().Be("MMMM dd, yyyy");
    }

    [Fact]
    public void FindText_ForNumberFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetNumberFormatKey(NumberFormat.CurrencyPattern, 3);
        _dbContext.Texts
            .Add(new() {
                Id = 1234,
                Key = key,
                ApplicationId = _application.Id,
                Culture = "en-US",
                Value = "0.000$",
            });
        _dbContext.SaveChanges();
        var provider = CreateProvider();

        // Act
        var result = provider.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("c3");
        subject.Value.Should().Be("0.000$");
    }

    [Fact]
    public void FindImage_ReturnsNull_WhenImageNotFound() {
        // Arrange
        var provider = CreateProvider();

        // Act
        var result = provider.FindImage("SomeImage");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindImage_ReturnsImageByteArray_WhenImageExists() {
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
        var result = provider.FindImage("ImageKey");

        // Assert
        var subject = result.Should().BeOfType<LocalizedImage>().Subject;
        subject.Label.Key.Should().Be("ImageKey");
        subject.Label.Value.Should().Be("Some image.");
        subject.Bytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }
}
