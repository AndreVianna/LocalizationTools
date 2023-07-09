namespace Common.UnitTests.Extensions;

public static class LoggerAssertionExtensions {

    public static void ShouldContainSingle<TLogger>(this ILogger<TLogger> logger, LogLevel logLevel, string message, EventId eventId = default) {
        var calls = logger.ReceivedCalls();
        var call = calls.Should().HaveCount(1).And.Subject.First();
        var arguments = call.GetArguments();
        arguments.Should().HaveCount(5);
        arguments[0].Should().BeOfType<LogLevel>().And.Be(logLevel);
        arguments[1].Should().BeOfType<EventId>().And.Be(eventId);
        arguments[2]!.ToString().Should().Be(message);
    }
}
