using System.Utilities;

namespace System;

public class CreateTests {
    private class TestClass { }

    [Fact]
    public void Instance_NoArgs_CreatesObjectOfType() {
        // Arrange & Act
        var instance = Create.Instance<TestClass>();

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestClass>();
    }

    private class TestClassWithArgs {
        public string Value { get; }

        public TestClassWithArgs(string value) {
            Value = value;
        }
    }

    [Fact]
    public void Instance_WithArgs_CreatesObjectOfTypeAndSetsValues() {
        // Arrange
        const string expectedValue = "Test";

        // Act
        var instance = Create.Instance<TestClassWithArgs>(expectedValue);

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<TestClassWithArgs>();
        instance.Value.Should().Be(expectedValue);
    }
}
