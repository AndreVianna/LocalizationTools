namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests : IDisposable {
    private readonly LocalizationDbContext _dbContext;
    private readonly PostgreSqlLocalizationProvider _provider;

    public PostgreSqlLocalizationProviderTests() {
        var builder = new DbContextOptionsBuilder<LocalizationDbContext>();
        builder.UseInMemoryDatabase($"LocalizationDbContext_{Guid.NewGuid()}");
        builder.EnableDetailedErrors();
        builder.EnableSensitiveDataLogging();
        _dbContext = new LocalizationDbContext(builder.Options);
        SeedApplication();

        _provider = CreateProvider(_defaultApplicationId);
    }

    public void Dispose() => _dbContext.Dispose();

    private PostgreSqlLocalizationProvider CreateProvider(Guid applicationId) {
        var options = new LocalizationOptions { ApplicationId = applicationId };
        return new(_dbContext, options);
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
        Action act = () => _provider.AsReader("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void AsReader_ReturnsProvider() {
        // Act
        var result = _provider.AsReader("en-CA");

        // Assert
        result.Should().BeSameAs(_provider);
    }

    [Fact]
    public void AsHandler_ReturnsProvider() {
        // Act
        var result = _provider.AsHandler("en-CA");

        // Assert
        result.Should().BeSameAs(_provider);
    }
}
