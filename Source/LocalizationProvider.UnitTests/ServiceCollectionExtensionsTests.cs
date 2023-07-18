namespace LocalizationProvider;

public class ServiceCollectionExtensionsTests {
    // ReSharper disable once ClassNeverInstantiated.Local
    private class DummyRepository : ILocalizationRepository {
        public IResourceReader AsReader(string culture) => throw new NotImplementedException();

        public IResourceRepository AsHandler(string culture) => throw new NotImplementedException();
    }

    [Fact]
    public void AddLocalizationProvider_RegisterServices() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddLocalizationProvider<DummyRepository, LocalizationRepositoryOptions>();

        // Assert
        result.Should().BeSameAs(services);
    }
}
