using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;

namespace PermissionApp.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Employee?> Employees { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionType> PermissionTypes { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PermissionType>().HasKey(x => x.Id);
        modelBuilder.Entity<PermissionType>().Property(x => x.Id).HasField("_id");
        modelBuilder.Entity<PermissionType>().Property(x => x.Name).HasField("_name");

        modelBuilder.Entity<Permission>().HasOne(x => x.Employee).WithMany(x => x.Permissions);
        modelBuilder.Entity<Permission>().HasOne(x => x.PermissionType);
        modelBuilder.Entity<Permission>().Property(x => x.Status).HasField("_status");
        
        base.OnModelCreating(modelBuilder);
    }
}