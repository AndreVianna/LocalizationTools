using System.ComponentModel.DataAnnotations;

namespace LocalizationProvider.PostgreSql;

public class LocalizedOption
{
    [Key]
    public required string ApplicationId { get; set; }
    [Key]
    public required string Culture { get; set; }
    [Key]
    public required string CategoryId { get; set; }
    [Key]
    public required int Index { get; set; }
    public required string LocalizedValue { get; set; }
}
