using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Suppress the pending model changes warning
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1, // Static ID
            FullName = "Admin User",
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin123"), // Static hashed password
            Role = "Admin",
            Gmail = "admin@example.com"
        });

        // You can seed other initial data here (e.g., blogs, roles, etc.)
    }
}
