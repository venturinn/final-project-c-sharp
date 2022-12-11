using Microsoft.EntityFrameworkCore;
using tryitter.Models;

namespace tryitter.Repository
{
    public interface ITryitterContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public int SaveChanges();
    }
}