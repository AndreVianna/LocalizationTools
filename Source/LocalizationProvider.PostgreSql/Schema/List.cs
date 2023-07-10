namespace LocalizationProvider.PostgreSql.Schema;

public class List : Resource {
    public int? LabelId { get; set; }
    public Text? Label { get; set; }
    public IList<ListItem> Items { get; set; } = new List<ListItem>();
}
