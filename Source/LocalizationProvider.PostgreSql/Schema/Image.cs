namespace LocalizationProvider.PostgreSql.Schema;

public class Image : Resource {
    public int? LabelId { get; set; }
    public Text? Label { get; set; }
    public byte[]? Bytes { get; set; }
}
