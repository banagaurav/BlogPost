using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }

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

        modelBuilder.Entity<Notification>()
          .HasOne(n => n.User) // A notification belongs to one user
          .WithMany(u => u.Notifications) // A user has many notifications
          .HasForeignKey(n => n.UserId) // Foreign key
          .OnDelete(DeleteBehavior.Cascade); // Cascade delete

    }
}
