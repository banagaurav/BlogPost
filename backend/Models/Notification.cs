using backend.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign key to User
    public string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; }
}
