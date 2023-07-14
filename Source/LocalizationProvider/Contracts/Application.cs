namespace LocalizationProvider.Contracts;

public class Application {
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string[] AvailableCultures { get; set; }
    public ICollection<LocalizedText> Texts { get; set; } = new HashSet<LocalizedText>();
    public ICollection<LocalizedList> Lists { get; set; } = new HashSet<LocalizedList>();
    public ICollection<LocalizedImage> Images { get; set; } = new HashSet<LocalizedImage>();
}
