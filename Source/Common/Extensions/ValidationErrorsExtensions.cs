namespace System.Extensions;

public static class ValidationErrorsExtensions {
    public static bool Contains(this IEnumerable<ValidationError> errors, string message)
        => errors.Any(error => error.MessageTemplate == message);

    public static void MergeWith(this ICollection<ValidationError> errors, IEnumerable<ValidationError> otherErrors) {
        foreach (var error in otherErrors.Where(e => !errors.Contains(e)))
            errors.Add(error);
    }
}
