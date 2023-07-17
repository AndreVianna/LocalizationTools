using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LocalizationProvider.PostgreSql.Schema;

[EntityTypeConfiguration(typeof(ListItem))]
public class ListItem : IEntityTypeConfiguration<ListItem> {
    public int ListId { get; set; }
    public List? List { get; set; }
    public int Index { get; set; }
    public int TextId { get; set; }
    public Text? Text { get; set; }

    public void Configure(EntityTypeBuilder<ListItem> builder) {
        builder.HasKey(e => new {
            e.ListId,
            e.Index
        });
        builder.HasOne(e => e.Text)
               .WithMany()
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new {
            e.ListId,
            e.TextId
        })
               .IsUnique();
        builder.Navigation(e => e.Text)
               .AutoInclude();
    }
}
