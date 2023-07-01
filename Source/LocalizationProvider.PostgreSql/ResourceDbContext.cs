namespace LocalizationProvider.PostgreSql;

internal class ResourceDbContext : DbContext
{
    public required DbSet<LocalizedString> Strings { get; set; }
    public required DbSet<LocalizedImage> Images { get; set; }
    public required DbSet<LocalizedOption> Options { get; set; }
}
