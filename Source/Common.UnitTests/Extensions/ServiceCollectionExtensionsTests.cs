namespace System.Extensions;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddDomainHandlers_RegisterHandlers() {
        var services = new ServiceCollection();

        var result = services.AddDefaultSystemComponents();

        result.Should().BeSameAs(services);
    }
}
