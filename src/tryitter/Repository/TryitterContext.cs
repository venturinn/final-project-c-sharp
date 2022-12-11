using Microsoft.EntityFrameworkCore;
using tryitter.Models;

namespace tryitter.Repository;

public class TryitterContext : DbContext, ITryitterContext
{
    public TryitterContext(DbContextOptions<TryitterContext> options) : base(options) { }
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Banco Local:
            //var connectionString = @"Server=127.0.0.1;Database=tryitter;User=SA;Password=senhaSuperSecreta.123;TrustServerCertificate=true;";
            var connectionString = @"Server=tcp:mysqlservertryitter.database.windows.net;Database=tryitter;User=tryitter;Password=trybe.123;Trusted_Connection=False;Encrypt=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        // BD Azure SQL Server - Alterar para variáveis de ambiente:
        // Servidor: mysqlservertryitter
        // Nome do banco: tryitter
        // logon: tryitter
        // Senha: trybe.123
        // Server=tcp:mysqlservertryitter.database.windows.net,1433;Initial Catalog=tryitter;Persist Security Info=False;User ID=tryitter;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
          .HasOne(u => u.User)
          .WithMany(p => p.Posts)
          .HasForeignKey(u => u.UserId);
    }
}