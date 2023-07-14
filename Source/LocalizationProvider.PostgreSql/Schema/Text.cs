using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalizationProvider.PostgreSql.Schema;

[EntityTypeConfiguration(typeof(Text))]
public class Text : Resource, IEntityTypeConfiguration<Text> {
    public string? Value { get; set; }

    public void Configure(EntityTypeBuilder<Text> builder) {

        builder.HasIndex(e => new { e.ApplicationId, e.Culture, ResourceId = e.Key })
               .IsUnique();
        builder.HasOne(e => e.Application)
               .WithMany(a => a.Texts)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
