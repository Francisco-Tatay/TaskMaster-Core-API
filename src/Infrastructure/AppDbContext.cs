using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskMasterPro.Domain;
using Task = TaskManagerPro.TaskMasterPro.Domain.Task;

namespace TaskManagerPro.TaskMasterPro.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> User { get; set; }
    public DbSet<Task> Task { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RefreshToken>().HasOne(rt => rt.User).WithMany().HasForeignKey(rt => rt.UserId);
    }
}