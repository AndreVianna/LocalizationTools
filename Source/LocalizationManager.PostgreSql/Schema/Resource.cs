namespace LocalizationManager.PostgreSql.Schema;

public class Resource {
    public int Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }
    public required string Culture { get; set; }
    public required string Key { get; set; }
}