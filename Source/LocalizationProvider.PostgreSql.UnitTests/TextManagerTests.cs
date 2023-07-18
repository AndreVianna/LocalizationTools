namespace LocalizationProvider.PostgreSql;

public sealed partial class PostgreSqlLocalizationProviderTests {
    [Fact]
    public void FindText_DateTimeProviderFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetDateTimeFormatKey(DateTimeFormat.LongDateTimePattern);
        SeedText(key, "MMMM dd, yyyy");

        // Act
        var result = _repository.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("dddd, dd MMMM yyyy HH:mm:ss");
        subject.Value.Should().Be("MMMM dd, yyyy");
    }

    [Fact]
    public void FindText_ForNumberFormat_ReturnsCorrectFormat_WhenResourceExists() {
        // Arrange
        var key = Keys.GetNumberFormatKey(NumberFormat.CurrencyPattern, 3);
        SeedText(key, "0.000$");

        // Act
        var result = _repository.FindText(key);

        // Assert
        var subject = result.Should().BeOfType<LocalizedText>().Subject;
        subject.Key.Should().Be("c3");
        subject.Value.Should().Be("0.000$");
    }

    [Fact]
    public void SetText_AddsLocalizedText() {
        const string key = "newText_key";
        var input = new LocalizedText(key, "Some text.");
        // Act
        _repository.SetText(input);

        // Assert
        _repository.FindText(key).Should().NotBeNull();
    }

    [Fact]
    public void SetText_WhenTextExists_UpdatesTextValue() {
        // Arrange
        const string key = "oldText_key";
        SeedText(key, "Old value");
        var input = new LocalizedText(key, "New value");

        // Act
        _repository.SetText(input);

        // Assert
        var result = _repository.FindText(key);
        result.Should().NotBeNull();
        result!.Value.Should().Be("New value");
    }

    private void SeedText(string key, string value) {
        _dbContext.Texts
                  .Add(new() {
                      Key = key,
                      ApplicationId = _application.Id,
                      Culture = _application.DefaultCulture,
                      Value = value,
                  });
        _dbContext.SaveChanges();
    }
}
