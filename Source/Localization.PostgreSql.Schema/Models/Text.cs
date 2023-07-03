namespace Localization.PostgreSql.Models;

public class Text : Resource {
    public required string Value { get; set; }
    public IList<ListItem> ItemLists { get; set; } = new List<ListItem>();
}