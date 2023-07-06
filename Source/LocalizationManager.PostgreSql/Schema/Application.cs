namespace LocalizationManager.PostgreSql.Schema;

public class Application {
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string AvailableCultures { get; set; }
    public IList<Text> Texts { get; set; } = new List<Text>();
    public IList<Image> Images { get; set; } = new List<Image>();
    public IList<List> Lists { get; set; } = new List<List>();
}
