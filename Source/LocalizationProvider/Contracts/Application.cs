using System.Results;
using System.Validation;

namespace LocalizationProvider.Contracts;

public class Application : IValidatable {
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string[] AvailableCultures { get; set; }
    public ICollection<LocalizedText> Texts { get; set; } = new HashSet<LocalizedText>();
    public ICollection<LocalizedList> Lists { get; set; } = new HashSet<LocalizedList>();
    public ICollection<LocalizedImage> Images { get; set; } = new HashSet<LocalizedImage>();

    public Result Validate(IDictionary<string, object?>? context = null) 
        => Result.Success();
}
