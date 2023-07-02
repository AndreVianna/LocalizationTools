namespace LocalizationProvider.PostgreSql.Models;

public class Image : Resource {
    public required byte[] Bytes { get; set; }
}