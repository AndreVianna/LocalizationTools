namespace LocalizationProvider.PostgreSql.Models;

public class Text : Resource {
    public required string Value { get; set; }
    public IReadOnlyList<ListOption> OptionList { get; set; } = new List<ListOption>();
}