namespace System.Results;

public sealed record ValidationError {
    public ValidationError(string messageTemplate, string source, params object?[] args) {
        MessageTemplate = Ensure.IsNotNullOrWhiteSpace(messageTemplate);
        Arguments = args.Prepend(Ensure.IsNotNullOrWhiteSpace(source)).ToArray();
    }

    public string MessageTemplate { get; }
    public object?[] Arguments { get; }

    public string Source => Arguments[0]!.ToString()!;
    public string Message => GetErrorMessage(MessageTemplate, Arguments);

    public bool Equals(ValidationError? other)
        => other is not null
           && MessageTemplate == other.MessageTemplate
           && Arguments.SequenceEqual(other.Arguments);

    public override int GetHashCode()
        => Arguments.Aggregate(MessageTemplate.GetHashCode(), HashCode.Combine);
}
