namespace LocalizationProvider;

public class LocalizerFactoryTests {
    // ReSharper disable once ClassNeverInstantiated.Local - Test class.
    private sealed class UnsupportedLocalizer : ILocalizer {
    }

    [Fact]
    public void Create_WithUnsupportedLocalizer_ThrowsUnsupportedException() {
        // Arrange
        var provider = Substitute.For<ILocalizationRepository>();
        var handler = Substitute.For<IResourceRepository>();
        provider.AsReader(Arg.Any<string>()).Returns(handler);
        var logger = Substitute.For<ILogger<TextResourceHandler>>();
        var factory = new LocalizerFactory(provider, logger.CreateFactory());

        // Act
        var action = () => factory.Create<UnsupportedLocalizer>("en-CA");

        // Assert
        action.Should().Throw<NotSupportedException>().WithMessage("Localizer of type 'UnsupportedLocalizer' is not supported.");
    }
}
