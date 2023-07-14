using Application = LocalizationProvider.PostgreSql.Schema.Application;

namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests {
    private static readonly Guid _defaultApplicationId = Guid.NewGuid();
    private static readonly Application _application = new() {
        Id = _defaultApplicationId,
        DefaultCulture = "en-US",
        Name = "SomeApplication",
        AvailableCultures = new[] { "en-CA", "fr-CA" },
    };

    private void SeedApplication() {
        _dbContext.Applications.Add(_application);
        _dbContext.SaveChanges();
    }
}
