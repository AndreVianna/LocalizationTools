namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests : IDisposable {
    private readonly LocalizationDbContext _dbContext;
    private readonly PostgreSqlLocalizationRepository _repository;

    public PostgreSqlLocalizationProviderTests() {
        var builder = new DbContextOptionsBuilder<LocalizationDbContext>();
        builder.UseInMemoryDatabase($"LocalizationDbContext_{Guid.NewGuid()}");
        builder.EnableDetailedErrors();
        builder.EnableSensitiveDataLogging();
        _dbContext = new LocalizationDbContext(builder.Options);
        SeedApplication();

        _repository = CreateProvider(_defaultApplicationId);
    }

    public void Dispose() => _dbContext.Dispose();

    private PostgreSqlLocalizationRepository CreateProvider(Guid applicationId) {
        var options = new LocalizationRepositoryOptions { ApplicationId = applicationId };
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
        Action act = () => _repository.AsReader("es-MX");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"Culture 'es-MX' is not available for application '{_application.Name}'.");
    }

    [Fact]
    public void AsReader_ReturnsProvider() {
        // Act
        var result = _repository.AsReader("en-CA");

        // Assert
        result.Should().BeSameAs(_repository);
    }

    [Fact]
    public void AsHandler_ReturnsProvider() {
        // Act
        var result = _repository.AsHandler("en-CA");

        // Assert
        result.Should().BeSameAs(_repository);
    }
}
