using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalizationProvider.PostgreSql.Schema;

[EntityTypeConfiguration(typeof(List))]
public class List : Resource, IEntityTypeConfiguration<List> {
    public ICollection<ListItem> Items { get; set; } = new HashSet<ListItem>();

    public void Configure(EntityTypeBuilder<List> builder) {
        builder.HasIndex(e => new {
                             e.ApplicationId,
                             e.Culture,
                             ResourceId = e.Key
                         })
               .IsUnique();
        builder.HasOne(e => e.Application)
               .WithMany(a => a.Lists)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Items)
               .WithOne(e => e.List)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(e => e.Items)
               .AutoInclude();
    }
}
