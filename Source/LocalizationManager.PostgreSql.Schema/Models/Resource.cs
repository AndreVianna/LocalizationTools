namespace LocalizationProvider.PostgreSql.Models;

public class Resource {
    public required int Id { get; set; }
    public required string ApplicationId { get; set; }
    public required string Culture { get; set; }
    public required string ResourceId { get; set; }
}