using LocalizationProvider.Contracts;

using DateTimeFormat = LocalizationProvider.Contracts.DateTimeFormat;

namespace LocalizationProvider.PostgreSql;

public sealed class PostgreSqlLocalizationProviderTests : IDisposable {
    private readonly ServiceCollection _serviceCollection;
    private readonly LocalizationDbContext _dbContext;
    private readonly Application _application;
    private readonly PostgreSqlLocalizationProvider _provider;

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
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _dbContext = serviceProvider.GetRequiredService<LocalizationDbContext>();
        _dbContext.Applications.Add(_application);
        _dbContext.SaveChanges();

        _provider = CreateProvider();
    }

    public void Dispose() => _provider.Dispose();

    private PostgreSqlLocalizationProvider CreateProvider(Guid? applicationId = null) {
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
    public void SetCulture_ThrowsException_WhenCultureNotAvailable() {
        // Act
        Action act = () => _provider.ForReadOnly("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void ForReadOnly_ReturnsProvider() {
        // Act
        var result = _provider.ForReadOnly("en-CA");

        // Assert
        result.Should().BeSameAs(_provider);
    }

    [Fact]
    public void ForUpdate_ReturnsProvider() {
        // Act
        var result = _provider.ForUpdate("en-CA");

        // Assert
        result.Should().BeSameAs(_provider);
    }

    [Fact]
    public void Dispose_DoNotThrowWhenCalledTwice()
        => _provider.Dispose();

    [Fact]
    public void FindText_DateTimeProviderFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetDateTimeFormatKey(DateTimeFormat.LongDateTimePattern);
        SeedText(key, "MMMM dd, yyyy");

        // Act
        var result = _provider.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("dddd, dd MMMM yyyy HH:mm:ss");
        subject.Value.Should().Be("MMMM dd, yyyy");
    }

    [Fact]
    public void FindText_ForNumberFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetNumberFormatKey(NumberFormat.CurrencyPattern, 3);
        SeedText(key, "0.000$");

        // Act
        var result = _provider.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("c3");
        subject.Value.Should().Be("0.000$");
    }

    [Fact]
    public void FindList_ReturnsNull_WhenListNotFound() {
        // Act
        var result = _provider.FindList("SomeList");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindList_ReturnsListItems_WhenListExists() {
        // Arrange
        SeedList("List_key", 2);

        // Act
        var result = _provider.FindList("List_key");

        // Assert
        var subject = result.Should().BeOfType<LocalizedList>().Subject;
        subject.Items.Should().HaveCount(2);
    }

    [Fact]
    public void FindImage_ReturnsNull_WhenImageNotFound() {
        // Act
        var result = _provider.FindImage("SomeImage");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindImage_ReturnsImageByteArray_WhenImageExists() {
        // Arrange
        SeedImage("image_key", new byte[] { 1, 2, 3, 4 });

        // Act
        var result = _provider.FindImage("image_key");

        // Assert
        var subject = result.Should().BeOfType<LocalizedImage>().Subject;
        subject.Bytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void SetText_AddsLocalizedText() {
        var input = new LocalizedText("text_key", "Some text.");
        // Act
        _provider.SetText(input);

        // Assert
        _provider.FindText("text_key").Should().NotBeNull();
    }

    [Fact]
    public void SetText_WhenTextExists_UpdatesTextValue() {
        // Arrange
        SeedText("text_key", "Old value");
        var input = new LocalizedText("text_key", "New value");

        // Act
        _provider.SetText(input);

        // Assert
        var result = _provider.FindText("text_key");
        result.Should().NotBeNull();
        result!.Value.Should().Be("New value");
    }

    [Fact]
    public void SetList_WhenListExists_UpdatesListValue() {
        // Arrange
        SeedList("list_key", 2);
        SeedText("item3_key", "Item 3");
        var input = new LocalizedList("list_key", new[] {
            new LocalizedText("item1_key", "Item 1"),
            new LocalizedText("item2_key", "Item 20"),
            new LocalizedText("item3_key", "Item 3"),
            new LocalizedText("item4_key", "Item 4")
        });

        // Act
        _provider.SetList(input);

        // Assert
        var result = _provider.FindList("list_key");
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(4);
    }

    [Fact]
    public void SetImage_WhenImageExists_UpdatesImageValue() {
        // Arrange
        SeedImage("image_key", new byte[] { 1, 2, 3, 4 });
        var input = new LocalizedImage("image_key", new byte[] { 5, 6, 7, 8 });

        // Act
        _provider.SetImage(input);

        // Assert
        var result = _provider.FindImage("image_key");
        result.Should().NotBeNull();
        result!.Bytes.Should().BeEquivalentTo(new byte[] { 5, 6, 7, 8 });
    }

    [Fact]
    public void SetList_AddsLocalizedList() {
        var input = new LocalizedList("list_key",
                                      new[] {
                                          new LocalizedText("item1_key", "Item 1"),
                                          new LocalizedText("item2_key", "Item 2"),
                                      });
        // Act
        _provider.SetList(input);

        // Assert
        _provider.FindList("list_key").Should().NotBeNull();
    }

    [Fact]
    public void SetList_WhenListExists_UpdateText() {
        SeedText("list_key", "List Name");
        var input = new LocalizedList("list_key",
                                      new[] {
                                          new LocalizedText("item1_key", "Item 1"),
                                          new LocalizedText("item2_key", "Item 2"),
                                      });
        // Act
        _provider.SetList(input);

        // Assert
        _provider.FindList("list_key").Should().NotBeNull();
    }

    [Fact]
    public void SetImage_AddsLocalizedImage() {
        var input = new LocalizedImage("image_key",
                                      new byte[] { 1, 2, 3, 4 });
        // Act
        _provider.SetImage(input);

        // Assert
        _provider.FindImage("image_key").Should().NotBeNull();
    }

    private void SeedText(string key, string value) {
        _dbContext.Texts
                  .Add(new() {
                       Key = key,
                       ApplicationId = _application.Id,
                       Culture = "en-US",
                       Value = value,
                   });
        _dbContext.SaveChanges();
    }

    private void SeedList(string key, int itemCount) {
        _dbContext.Lists
                  .Add(new() {
                       Key = key,
                       ApplicationId = _application.Id,
                       Culture = "en-US",
                       Items = Enumerable
                              .Range(0, itemCount)
                              .Select(i => new ListItem {
                                   Index = i,
                                   Text = new() {
                                       ApplicationId = _application.Id,
                                       Culture = "en-US",
                                       Key = $"item{i + 1}_key",
                                       Value = $"Item {i + 1}"
                                   },
                               }).ToArray(),
                   });
        _dbContext.SaveChanges();
    }

    private void SeedImage(string key, byte[] bytes) {
        _dbContext.Images
                  .Add(new() {
                       Key = key,
                       ApplicationId = _application.Id,
                       Culture = "en-US",
                       Bytes = bytes,
                   });
        _dbContext.SaveChanges();
    }

}
