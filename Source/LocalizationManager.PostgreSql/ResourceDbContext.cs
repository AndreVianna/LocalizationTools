namespace LocalizationManager.PostgreSql;

public class ResourceDbContext : DbContext
{
    public required DbSet<Application> Applications { get; set; }
    public required DbSet<Text> Texts { get; set; }
    public required DbSet<Image> Images { get; set; }
    public required DbSet<List> Lists { get; set; }
    public required DbSet<ListItem> ListItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql();
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Text>()
                    .HasIndex(e => new {
                                  e.ApplicationId,
                                  e.Culture,
                                  ResourceId = e.Key
                              })
                    .IsUnique();
        modelBuilder.Entity<Text>()
                    .HasOne(e => e.Application)
                    .WithMany(a => a.Texts)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Image>()
                    .HasIndex(e => new {
                         e.ApplicationId,
                         e.Culture,
                         ResourceId = e.Key
                     })
                    .IsUnique();
        modelBuilder.Entity<Image>()
                    .HasOne(e => e.Application)
                    .WithMany(a => a.Images)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<List>()
                    .HasIndex(e => new {
                         e.ApplicationId,
                         e.Culture,
                         ResourceId = e.Key
                     })
                    .IsUnique();
        modelBuilder.Entity<List>()
                    .HasMany(e => e.Items)
                    .WithOne(e => e.List)
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<List>()
                    .HasOne(e => e.Application)
                    .WithMany(a => a.Lists)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ListItem>()
                    .HasKey(e => new {
                         e.ListId,
                         e.Index
                     });
        modelBuilder.Entity<ListItem>()
                    .HasIndex(e => new {
                         e.ListId,
                         e.Value
                     })
                    .IsUnique();
    }
}
