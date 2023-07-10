namespace LocalizationProvider.PostgreSql;

internal class LocalizationDbContext : DbContext {
    public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
        : base(options) {
    }

    public required DbSet<Application> Applications { get; set; }
    public required DbSet<Text> Texts { get; set; }
    public required DbSet<Image> Images { get; set; }
    public required DbSet<List> Lists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Application>()
            .HasIndex(e => new { e.Name, })
            .IsUnique();
        modelBuilder.Entity<Application>()
            .Property(e => e.AvailableCultures)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<string[]>(v, JsonSerializerOptions.Default) ?? Array.Empty<string>());

        modelBuilder.Entity<Text>()
                    .HasIndex(e => new { e.ApplicationId, e.Culture, ResourceId = e.Key })
                    .IsUnique();
        modelBuilder.Entity<Text>()
                    .HasOne(e => e.Application)
                    .WithMany(a => a.Texts)
                    .IsRequired()
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
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Image>()
                    .HasOne(e => e.Label)
                    .WithMany()
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<List>()
                    .HasIndex(e => new {
                        e.ApplicationId,
                        e.Culture,
                        ResourceId = e.Key
                    })
                    .IsUnique();
        modelBuilder.Entity<List>()
                    .HasOne(e => e.Label)
                    .WithMany()
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<List>()
                    .HasOne(e => e.Application)
                    .WithMany(a => a.Lists)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<List>()
                    .HasMany(e => e.Items)
                    .WithOne(e => e.List)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<List>()
                    .Navigation(e => e.Items)
                    .AutoInclude();

        modelBuilder.Entity<ListItem>()
                    .HasKey(e => new {
                        e.ListId,
                        e.Index
                    });
        modelBuilder.Entity<ListItem>()
                    .HasOne(e => e.Text)
                    .WithMany()
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ListItem>()
                    .HasIndex(e => new {
                        e.ListId,
                        e.TextId
                    })
                    .IsUnique();
        modelBuilder.Entity<ListItem>()
                    .Navigation(e => e.Text)
                    .AutoInclude();
    }
}
