namespace Localization.PostgreSql.Models;

public class ListItem {
    public int ListId { get; set; }
    public List? List { get; set; }
    public int ItemId { get; set; }
    public Text? Item { get; set; }
    public int Index { get; set; }
}