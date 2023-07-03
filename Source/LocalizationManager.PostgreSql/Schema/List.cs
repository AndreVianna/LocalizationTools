namespace LocalizationManager.PostgreSql.Schema;

public class List : Resource {
    public required string Name { get; set; }
    public IList<ListItem> Items { get; set; } = new List<ListItem>();
}