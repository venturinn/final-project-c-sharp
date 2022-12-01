using Microsoft.EntityFrameworkCore;
using tryitter.Models;
// using Microsoft.Extensions.DependencyInjection;

namespace tryitter.Repository;

public class TryitterContext : DbContext
{
    public TryitterContext(DbContextOptions<TryitterContext> options) : base(options) { }
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = @"Server=127.0.0.1;Database=tryitter;User=SA;Password=senhaSuperSecreta.123;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
          .HasOne(u => u.User)
          .WithMany(p => p.Posts)
          .HasForeignKey(u => u.UserId);
    }
}