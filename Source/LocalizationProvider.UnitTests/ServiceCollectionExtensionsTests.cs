namespace LocalizationProvider;

public class ServiceCollectionExtensionsTests {
    // ReSharper disable once ClassNeverInstantiated.Local
    private class DummyFactory : ILocalizationRepositoryFactory {
        public ILocalizationRepository CreateFor(string culture) => throw new NotImplementedException();
    }

    [Fact]
    public void AddLocalizationProvider_RegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLocalizationProvider<DummyFactory, LocalizationRepositoryOptions>();

        // Assert
        result.Should().BeSameAs(services);
    }
}
