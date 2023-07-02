namespace LocalizationProvider.PostgreSql.Models;

public class Resource {
    public required int Id { get; set; }
    public required Guid ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    public required string Culture { get; set; }
    public required string ResourceId { get; set; }
}