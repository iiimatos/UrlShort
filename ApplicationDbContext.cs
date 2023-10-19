using Microsoft.EntityFrameworkCore;
using UrlShort.Models;
using UrlShort.Services;


namespace UrlShort;

public class ApplicationDbContext : DbContext
{
  public virtual DbSet<UrlManagement> Urls { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlManagement>(builder => {
          builder.Property(u => u.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
          builder.HasIndex(u => u.Code).IsUnique();
        });
    }
}