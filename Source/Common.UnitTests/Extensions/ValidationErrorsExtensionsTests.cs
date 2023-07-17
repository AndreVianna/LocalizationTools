namespace System.Extensions;

public class ValidationErrorsExtensionsTests {
    [Fact]
    public void Contains_Returns() {
        // Arrange
        var subject = new List<ValidationError> {
            new("Some message.", "Source"),
            new("Other message.", "Source")
        };

        // Assert
        subject.Contains("Some message.").Should().BeTrue();
        subject.Contains("Missing message.").Should().BeFalse();
    }

    [Fact]
    public void MergeWith_ReturnsMergedList() {
        // Arrange
        var subject1 = new List<ValidationError> {
            new("Some message 1.", "Source 1"),
            new("Some message 2.", "Source 1"),
            new("Some message 1.", "Source 2"),
        };
        var subject2 = new List<ValidationError> {
            new("Some message 1.", "Source 1"),
            new("Some message 2.", "Source 2"),
            new("Some message 3.", "Source 2"),
        };

        // Act
        subject1.MergeWith(subject2);

        // Assert
        subject1.Should().BeEquivalentTo(new ValidationError[] {
            new("Some message 1.", "Source 1"),
            new("Some message 1.", "Source 2"),
            new("Some message 2.", "Source 1"),
            new("Some message 2.", "Source 2"),
            new("Some message 3.", "Source 2"),
        });
    }
}
