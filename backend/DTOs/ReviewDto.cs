public class ReviewDto
{
    public int BlogPostId { get; set; } // Blog post being reviewed
    public string Content { get; set; } // Review content
    public int Rating { get; set; }     // Rating value (1-5)
}