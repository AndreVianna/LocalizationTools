namespace Localization.PostgreSql.Models;

public class List : Resource {
    public IList<ListItem> ListItems { get; set; } = new List<ListItem>();
}