namespace LocalizationProvider.PostgreSql.Models;

public class Application {
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string AvailableCultures { get; set; }
    public IReadOnlyList<Text> Texts { get; set; } = new List<Text>();
    public IReadOnlyList<Image> Images { get; set; } = new List<Image>();
    public IReadOnlyList<List> Lists { get; set; } = new List<List>();
}