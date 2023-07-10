namespace LocalizationProvider.PostgreSql.Schema;

public class ListItem {
    public int ListId { get; set; }
    public List List { get; set; } = null!;
    public int Index { get; set; }
    public int TextId { get; set; }
    public Text Text { get; set; } = null!;
}
