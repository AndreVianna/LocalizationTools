using LocalizationProvider.PostgreSql.Extensions;

namespace LocalizationProvider.PostgreSql;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddLocalizationProvider_RegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddPostgreSqlLocalizationProvider();

        // Assert
        result.Should().BeSameAs(services);
    }
}
