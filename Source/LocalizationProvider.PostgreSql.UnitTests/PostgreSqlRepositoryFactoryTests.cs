using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

public sealed class PostgreSqlRepositoryFactoryTests : IDisposable {
    private static readonly Guid _defaultApplicationId = Guid.NewGuid();
    private static readonly Application _application = new() {
        Id = _defaultApplicationId,
        DefaultCulture = "en-CA",
        Name = "Some Application",
        AvailableCultures = new[] { "en-CA", "fr-CA" },
    };

    private readonly LocalizationDbContext _dbContext;
    private readonly PostgreSqlLocalizationRepositoryFactory _factory;

    public PostgreSqlRepositoryFactoryTests() {
        var builder = new DbContextOptionsBuilder<LocalizationDbContext>();
        builder.UseInMemoryDatabase($"LocalizationDbContext_{Guid.NewGuid()}");
        builder.EnableDetailedErrors();
        builder.EnableSensitiveDataLogging();
        _dbContext = new(builder.Options);
        SeedApplication();

        _factory = CreateFactory(_defaultApplicationId);
    }

    public void Dispose() => _dbContext.Dispose();

    private PostgreSqlLocalizationRepositoryFactory CreateFactory(Guid applicationId) {
        var options = new LocalizationRepositoryOptions { ApplicationId = applicationId };
        return new(_dbContext, options);
    }

    private void SeedApplication(Application? application = null) {
        _dbContext.Applications.Add(application ?? _application);
        _dbContext.SaveChanges();
    }

    [Fact]
    public void Constructor_ThrowsException_WhenApplicationDoesNotExist() {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        Action act = () => CreateFactory(invalidId);

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage($"An Application with id '{invalidId}' was not found.");
    }

    [Fact]
    public void CreateResourceRepository_ReturnsRepository() {
        // Act
        var result = _factory.CreateFor("en-CA");

        // Assert
        result.Should().BeOfType<PostgreSqlLocalizationRepository>();
    }

    [Fact]
    public void CreateResourceRepository_WithUnsupportedCulture_ReturnsRepository() {
        // Act
        Action act = () => _factory.CreateFor("es-MX");

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage($"Culture 'es-MX' is not available for application 'Some Application'.");
    }
}
