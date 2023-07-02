using LocalizationProvider.PostgreSql.Models;

namespace LocalizationProvider.PostgreSql;

public class ResourceDbContext : DbContext
{
    public required DbSet<Text> Texts { get; set; }
    public required DbSet<Image> Images { get; set; }
    public required DbSet<List> Lists { get; set; }
    public required DbSet<ListOption> ListOptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql();
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Text>()
                    .HasIndex(e => new {
                                  e.ApplicationId,
                                  e.Culture,
                                  e.ResourceId
                              })
                    .IsUnique();
        modelBuilder.Entity<Text>()
                    .HasMany(e => e.OptionList)
                    .WithOne(e => e.Option)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Image>()
                    .HasIndex(e => new {
                         e.ApplicationId,
                         e.Culture,
                         e.ResourceId
                     })
                    .IsUnique();

        modelBuilder.Entity<List>()
                    .HasIndex(e => new {
                         e.ApplicationId,
                         e.Culture,
                         e.ResourceId
                     })
                    .IsUnique();
        modelBuilder.Entity<List>()
                    .HasMany(e => e.ListOptions)
                    .WithOne(e => e.List)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ListOption>()
                    .HasKey(e => new {
                         e.ListId,
                         e.OptionId
                     });
        modelBuilder.Entity<ListOption>()
                    .HasIndex(e => new {
                         e.ListId,
                         e.Index
                     })
                    .IsUnique();
    }
}
