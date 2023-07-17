namespace System.Constants;

public static class Constants {
    public static class ErrorMessages {
        public static string GetInvertedErrorMessage(string message, params object?[] args)
            => GetErrorMessage(InvertMessage(message), args);

        public static string GetErrorMessage(string message, params object?[] args)
            => string.Format(message, args);

        public static string InvertMessage(string message) => message switch {
            _ when message.Contains(" cannot ") => message.Replace(" cannot ", " must "),
            _ when message.Contains(" must ") => message.Replace(" must ", " cannot "),
            _ when message.Contains(" is not ") => message.Replace(" is not ", " is "),
            _ when message.Contains(" is ") => message.Replace(" is ", " is not "),
            _ => message
        };

        public const string MustBeEmpty = "'{0}' must be empty.";
        public const string MustBeEmptyOrWhitespace = "'{0}' must be empty or whitespace.";
        public const string MustBeNull = "'{0}' must be null.";
        public const string MustContainNull = "'{0}' must contain null item(s).";
        public const string MustContainNullOrEmpty = "'{0}' must contain null or empty string(s).";
        public const string MustContainNullOrWhitespace = "'{0}' must contain null or whitespace string(s).";
        public const string MustBeOfType = "'{0}' must be of type '{1}'. Found: '{2}'.";
    }
}
