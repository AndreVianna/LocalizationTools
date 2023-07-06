namespace LocalizationManager.PostgreSql.Schema;

public class ListItem {
    public int ListId { get; set; }
    public List? List { get; set; }
    public required int Index { get; set; }
    public required string Value { get; set; }
}
