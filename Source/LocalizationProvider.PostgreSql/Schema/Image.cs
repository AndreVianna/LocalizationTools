namespace LocalizationProvider.PostgreSql.Schema;

public class Image : Resource {
    public required byte[] Bytes { get; set; }
}
