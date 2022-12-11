using Microsoft.EntityFrameworkCore;
using tryitter.Models;
using tryitter.Repository;

namespace video_portal.Test;

public class VideoPortalTestContext : DbContext, ITryitterContext
{
    public VideoPortalTestContext(DbContextOptions<VideoPortalTestContext> options)
            : base(options)
    { }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<User> Users { get; set; }
}