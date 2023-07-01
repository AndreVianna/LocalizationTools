using System.ComponentModel.DataAnnotations;

namespace LocalizationProvider.PostgreSql;

public class LocalizedImage
{
    [Key]
    public required string ApplicationId { get; set; }
    [Key]
    public required string Culture { get; set; }
    [Key]
    public required string ImageId { get; set; }
    public required byte[] Bytes { get; set; }
}