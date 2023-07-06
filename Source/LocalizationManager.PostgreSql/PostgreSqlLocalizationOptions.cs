namespace LocalizationManager.PostgreSql;

public record PostgreSqlLocalizationOptions : LocalizationOptions {
    [Required]
    public required string ConnectionString { get; init; }
}
