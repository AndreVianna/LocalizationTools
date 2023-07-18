using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlResourceRepositoryTests : IDisposable {
    private static readonly Guid _defaultApplicationId = Guid.NewGuid();
    private static readonly Application _application = new() {
        Id = _defaultApplicationId,
        DefaultCulture = "en-CA",
        Name = "SomeApplication",
        AvailableCultures = new[] { "en-CA", "fr-CA" },
    };

    private readonly LocalizationDbContext _dbContext;
    private readonly ILocalizationRepository _repository;

    public PostgreSqlResourceRepositoryTests() {
        var builder = new DbContextOptionsBuilder<LocalizationDbContext>();
        builder.UseInMemoryDatabase($"LocalizationDbContext_{Guid.NewGuid()}");
        builder.EnableDetailedErrors();
        builder.EnableSensitiveDataLogging();
        _dbContext = new(builder.Options);
        SeedApplication();

        var options = new LocalizationRepositoryOptions { ApplicationId = _application.Id };
        var factory = new PostgreSqlLocalizationLocalizationRepositoryFactory(_dbContext, options);
        _repository = factory.CreateFor("en-CA");
    }

    public void Dispose() => _dbContext.Dispose();
}
