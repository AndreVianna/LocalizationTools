namespace LocalizationProvider;

public class ServiceCollectionExtensionsTests {
    // ReSharper disable once ClassNeverInstantiated.Local
    private class DummyProvider : ILocalizationProvider {
        public ILocalizationReader AsReader(string culture) => throw new NotImplementedException();

        public ILocalizationHandler AsHandler(string culture) => throw new NotImplementedException();
    }

    [Fact]
    public void AddLocalizationProvider_RegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLocalizationProvider<DummyProvider, LocalizationOptions>();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddLocalizationHandler_RegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLocalizationHandler<DummyProvider, LocalizationOptions>();

        // Assert
        result.Should().BeSameAs(services);
    }
}
