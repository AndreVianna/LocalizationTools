namespace LocalizationProvider.PostgreSql.Models;

public class ListOption {
    public required int ListId { get; set; }
    public List List { get; set; } = null!;
    public required int OptionId { get; set; }
    public Text Option { get; set; } = null!;
    public required int Index { get; set; }
}