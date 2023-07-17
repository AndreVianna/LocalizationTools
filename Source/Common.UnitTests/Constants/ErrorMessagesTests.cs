namespace System.Constants;

public class ErrorMessagesTests {
    [Theory]
    [InlineData("'Abc' cannot be null.", "'Abc' must be null.")]
    [InlineData("'Abc' must be null.", "'Abc' cannot be null.")]
    [InlineData("'Abc' is null.", "'Abc' is not null.")]
    [InlineData("'Abc' is not null.", "'Abc' is null.")]
    [InlineData("'Abc' something different.", "'Abc' something different.")]
    public void InvertMessage_ReturnsInvertedMessage(string message, string expectedMessage) {
        // Act
        var result = Constants.ErrorMessages.GetInvertedErrorMessage(message);

        // Assert
        result.Should().Be(expectedMessage);
    }
}