using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalizationProvider.PostgreSql.Schema;

[EntityTypeConfiguration(typeof(Image))]
public class Image : Resource, IEntityTypeConfiguration<Image> {
    public required byte[] Bytes { get; set; }

    public void Configure(EntityTypeBuilder<Image> builder) {
        builder.HasIndex(e => new {
                         e.ApplicationId,
                         e.Culture,
                         ResourceId = e.Key
                     })
               .IsUnique();
        builder.HasOne(e => e.Application)
               .WithMany(a => a.Images)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
