namespace LocalizationProvider.PostgreSql.Schema;

public class List : Resource {
    public string? Label { get; set; }
    public IList<ListItem> Items { get; set; } = new List<ListItem>();
}
