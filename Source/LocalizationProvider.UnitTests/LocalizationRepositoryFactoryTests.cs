using LocalizationProvider.Models;

namespace LocalizationProvider;

public class LocalizationRepositoryFactoryTests {
    // ReSharper disable once ClassNeverInstantiated.Local - Test class.
    private sealed class UnsupportedLocalizer : ITypedLocalizer {
        public static ResourceType Type => ResourceType.Image;
    }

    [Fact]
    public void Create_WithUnsupportedLocalizer_ThrowsUnsupportedException() {
        // Arrange
        var provider = Substitute.For<ILocalizationRepositoryFactory>();
        var repository = Substitute.For<ILocalizationRepository>();
        provider.CreateFor(Arg.Any<string>()).Returns(repository);
        var logger = Substitute.For<ILogger<TextResourceHandler>>();
        var factory = new LocalizerFactory(provider, logger.CreateFactory());

        // Act
        var action = () => factory.Create<UnsupportedLocalizer>("en-CA");

        // Assert
        action.Should().Throw<NotSupportedException>().WithMessage("A localizer of type 'UnsupportedLocalizer' is not supported.");
    }
}
