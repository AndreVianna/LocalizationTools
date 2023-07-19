using System.Validation;

namespace LocalizationProvider.Contracts;

public class Application : IValidatable {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string[] AvailableCultures { get; set; }
    public ICollection<LocalizedText> Texts { get; set; } = new HashSet<LocalizedText>();
    public ICollection<LocalizedList> Lists { get; set; } = new HashSet<LocalizedList>();
    public ICollection<LocalizedImage> Images { get; set; } = new HashSet<LocalizedImage>();

    public Result Validate(IDictionary<string, object?>? context = null) {
        var result = Result.Success();
        result = ValidateName(result);
        result = ValidateDefaultCulture(result);
        result = ValidateAvailableCultures(result);
        return result;
    }

    private Result ValidateName(Result result) {
        if (string.IsNullOrWhiteSpace(Name))
            result += new ValidationError($"'{nameof(Name)}' cannot be null or whitespace.", nameof(Name));
        return result;
    }

    private Result ValidateDefaultCulture(Result result) {
        if (string.IsNullOrWhiteSpace(DefaultCulture))
            result += new ValidationError($"'{nameof(DefaultCulture)}' cannot be null or whitespace.", nameof(DefaultCulture));
        if (!AvailableCultures.Contains(DefaultCulture))
            result += new ValidationError($"'{nameof(DefaultCulture)}' must be one of the available cultures. Available cultures: '{0}'. Default culture: '{1}'.", nameof(DefaultCulture), string.Join(", ", AvailableCultures), DefaultCulture);
        return result;
    }

    private Result ValidateAvailableCultures(Result result) {
        if (AvailableCultures.Length == 0)
            result += new ValidationError($"{nameof(AvailableCultures)} must contain at least one culture. Found empty.", nameof(AvailableCultures));
        if (AvailableCultures.Any(string.IsNullOrWhiteSpace))
            result += new ValidationError($"{nameof(AvailableCultures)} cannot contain a null or empty item. Found '[{string.Join(", ", AvailableCultures.Select(i => string.IsNullOrWhiteSpace(i) ? "<Empty>" : i))}]'.", nameof(AvailableCultures));
        return result;
    }
}
