using backend.Models;

public class Review
{
    public int Id { get; set; }                // Primary key
    public int BlogPostId { get; set; }        // Foreign key to BlogPost
    public Blog Blog { get; set; }     // Navigation property
    public int UserId { get; set; }            // Foreign key to User
    public User User { get; set; }             // Navigation property
    public string Content { get; set; }        // User's review content
    public int Rating { get; set; }            // Rating value (e.g., 1-5)
    public DateTime CreatedAt { get; set; }    // When the rating was added
}
