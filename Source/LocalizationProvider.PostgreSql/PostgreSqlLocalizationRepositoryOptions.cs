namespace LocalizationProvider.PostgreSql;

public record PostgreSqlLocalizationRepositoryOptions : LocalizationRepositoryOptions {
    [Required]
    public required string ConnectionString { get; init; }
}
