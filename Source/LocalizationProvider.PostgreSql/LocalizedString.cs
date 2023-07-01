using System.ComponentModel.DataAnnotations;

namespace LocalizationProvider.PostgreSql;

public class LocalizedString
{
    [Key]
    public required string ApplicationId { get; set; }
    [Key]
    public required string Culture { get; set; }
    [Key]
    public required string StringId { get; set; }
    public required string LocalizedValue { get; set; }
}