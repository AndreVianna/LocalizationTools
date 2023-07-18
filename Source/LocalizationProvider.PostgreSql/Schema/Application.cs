using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalizationProvider.PostgreSql.Schema;

[EntityTypeConfiguration(typeof(Application))]
public class Application : IEntityTypeConfiguration<Application> {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DefaultCulture { get; set; }
    public required string[] AvailableCultures { get; set; }
    public ICollection<Text> Texts { get; set; } = new HashSet<Text>();
    public ICollection<List> Lists { get; set; } = new HashSet<List>();
    public ICollection<Image> Images { get; set; } = new HashSet<Image>();

    public void Configure(EntityTypeBuilder<Application> builder) {
        builder.HasIndex(e => new { e.Name, })
               .IsUnique();
        builder.Property(e => e.AvailableCultures)
               .HasConversion(v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                              v => JsonSerializer.Deserialize<string[]>(v, JsonSerializerOptions.Default) ?? Array.Empty<string>());
    }
}
