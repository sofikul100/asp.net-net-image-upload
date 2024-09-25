
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get;set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
      
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        // modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}