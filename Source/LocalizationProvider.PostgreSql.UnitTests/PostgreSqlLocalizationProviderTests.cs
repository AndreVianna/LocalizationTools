using LocalizationProvider.Contracts;

using DateTimeFormat = LocalizationProvider.Contracts.DateTimeFormat;

namespace LocalizationProvider.PostgreSql;

public class PostgreSqlLocalizationProviderTests {
    private readonly ServiceCollection _serviceCollection;
    private readonly LocalizationDbContext _dbContext;
    private readonly Application _application;

    public PostgreSqlLocalizationProviderTests() {
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
        var serviceManager = _serviceCollection.BuildServiceProvider();
        _dbContext = serviceManager.GetRequiredService<LocalizationDbContext>();
        _dbContext.Applications.Add(_application);
        _dbContext.SaveChanges();
    }

    private PostgreSqlLocalizationProvider CreateManager(Guid? applicationId = null) {
        _serviceCollection.AddSingleton<LocalizationOptions>(_ => new() { ApplicationId = applicationId ?? _application.Id });
        var serviceManager = _serviceCollection.BuildServiceProvider();
        return new(serviceManager);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenApplicationDoesNotExist() {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        Action act = () => CreateManager(invalidId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Application with id '{invalidId}' not found.");
    }

    [Fact]
    public void ForReadOnly_ThrowsException_WhenCultureNotAvailable() {
        // Arrange
        var manager = CreateManager();

        // Act
        Action act = () => manager.ForReadOnly("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void ForUpdate_ThrowsException_WhenCultureNotAvailable() {
        // Arrange
        var manager = CreateManager();

        // Act
        Action act = () => manager.ForUpdate("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void FindText_DateTimeManagerFormat_ReturnsCorrectFormat_WhenResourceExists() {
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
        var manager = CreateManager();

        // Act
        var result = manager.FindText(key);

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
        var manager = CreateManager();

        // Act
        var result = manager.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("c3");
        subject.Value.Should().Be("0.000$");
    }

    [Fact]
    public void FindList_ReturnsNull_WhenListNotFound() {
        // Arrange
        var manager = CreateManager();

        // Act
        var result = manager.FindList("SomeList");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindList_ReturnsListItems_WhenListExists() {
        // Arrange
        _dbContext.Lists
                  .Add(new() {
                       Id = 1234,
                       Key = "ListKey",
                       ApplicationId = _application.Id,
                       Culture = "en-US",
                       Items = new[] {
                           new ListItem {
                               Index = 0,
                               Text = new() {
                                   ApplicationId = _application.Id,
                                   Culture = "en-US",
                                   Key = "Item1",
                                   Value = "Item 1"
                               },
                           },
                           new ListItem {
                               Index = 1,
                               Text = new() {
                                   ApplicationId = _application.Id,
                                   Culture = "en-US",
                                   Key = "Item2",
                                   Value = "Item 2"
                               },
                           },
                       },
                   });
        _dbContext.SaveChanges();
        var manager = CreateManager();

        // Act
        var result = manager.FindList("ListKey");

        // Assert
        var subject = result.Should().BeOfType<LocalizedList>().Subject;
        subject.Items.Should().HaveCount(2);
    }

    [Fact]
    public void FindImage_ReturnsNull_WhenImageNotFound() {
        // Arrange
        var manager = CreateManager();

        // Act
        var result = manager.FindImage("SomeImage");

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
                Bytes = new byte[] { 1, 2, 3, 4 },
            });
        _dbContext.SaveChanges();
        var manager = CreateManager();

        // Act
        var result = manager.FindImage("ImageKey");

        // Assert
        var subject = result.Should().BeOfType<LocalizedImage>().Subject;
        subject.Bytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }
}
