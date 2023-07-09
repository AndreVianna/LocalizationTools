namespace LocalizationProvider.PostgreSql.Schema;

public class Image : Resource {
    public string? Label { get; set; }
    public byte[]? Bytes { get; set; }
}
