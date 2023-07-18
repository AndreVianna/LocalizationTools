using System.Results;
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
        if (string.IsNullOrWhiteSpace(Name))
            result += new ValidationError($"'{nameof(Name)}' cannot be null or whitespace.", nameof(Name));

        if (string.IsNullOrWhiteSpace(DefaultCulture))
            result += new ValidationError($"'{nameof(DefaultCulture)}' cannot be null or whitespace.", nameof(DefaultCulture));

        if (AvailableCultures.Length == 0)
            result += new ValidationError($"{nameof(AvailableCultures)} must contain at least one culture. Found empty.", nameof(AvailableCultures));
        if (!AvailableCultures.Contains(DefaultCulture))
            result += new ValidationError($"'{nameof(DefaultCulture)}' must be one of the available cultures. Available cultures: '{0}'. Default culture: '{1}'.", nameof(DefaultCulture), string.Join(", ", AvailableCultures), DefaultCulture);

        return result;
    }
}
