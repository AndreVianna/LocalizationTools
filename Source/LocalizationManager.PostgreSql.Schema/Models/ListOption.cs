namespace LocalizationProvider.PostgreSql.Models;

public class ListOption {
    public required int ListId { get; set; }
    public required List List { get; set; }
    public required int OptionId { get; set; }
    public required Text Option { get; set; }
    public required int Index { get; set; }
}