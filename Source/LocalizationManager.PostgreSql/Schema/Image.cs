namespace LocalizationManager.PostgreSql.Schema;

public class Image : Resource {
    public required string Label { get; set; }
    public required byte[] Bytes { get; set; }
}