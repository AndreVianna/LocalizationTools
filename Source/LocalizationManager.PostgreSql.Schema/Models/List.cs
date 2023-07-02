namespace LocalizationProvider.PostgreSql.Models;

public class List : Resource {
    public IReadOnlyList<ListOption> ListOptions { get; set; } = new List<ListOption>();
}