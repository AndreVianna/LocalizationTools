namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests {
    [Fact]
    public void FindImage_ReturnsNull_WhenImageNotFound() {
        // Act
        var result = _provider.FindImage("SomeImage");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindImage_ReturnsImageByteArray_WhenImageExists() {
        // Arrange
        SeedImage("image_key", new byte[] { 1, 2, 3, 4 });

        // Act
        var result = _provider.FindImage("image_key");

        // Assert
        var subject = result.Should().BeOfType<LocalizedImage>().Subject;
        subject.Bytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void SetImage_AddsLocalizedImage() {
        const string key = "newImage_key";
        var input = new LocalizedImage(key,
                                       new byte[] { 1, 2, 3, 4 });
        // Act
        _provider.SetImage(input);

        // Assert
        _provider.FindImage(key).Should().NotBeNull();
    }

    [Fact]
    public void SetImage_WhenImageExists_UpdatesImageValue() {
        // Arrange
        const string key = "oldImage_key";
        SeedImage(key, new byte[] { 1, 2, 3, 4 });
        var input = new LocalizedImage(key, new byte[] { 5, 6, 7, 8 });

        // Act
        _provider.SetImage(input);

        // Assert
        var result = _provider.FindImage(key);
        result.Should().NotBeNull();
        result!.Bytes.Should().BeEquivalentTo(new byte[] { 5, 6, 7, 8 });
    }

    private void SeedImage(string key, byte[] bytes) {
        _dbContext.Images
                  .Add(new() {
                       Key = key,
                       ApplicationId = _application.Id,
                       Culture = "en-US",
                       Bytes = bytes,
                   });
        _dbContext.SaveChanges();
    }
}