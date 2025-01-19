using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public IActionResult AddReview([FromBody] ReviewDto reviewDto)
    {
        try
        {
            // Ensure the 'id' claim exists
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return Unauthorized(new { Message = "User ID claim is missing from the token." });
            }

            // Parse user ID
            if (!int.TryParse(idClaim.Value, out var userId))
            {
                return BadRequest(new { Message = "Invalid user ID in token." });
            }

            // Validate the rating value
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                return BadRequest(new { Message = "Rating value must be between 1 and 5." });
            }

            // Check if the blog post exists
            var blogPost = _context.Blogs.Find(reviewDto.BlogPostId);
            if (blogPost == null)
            {
                return NotFound(new { Message = "Blog post not found." });
            }

            // Check if the user has already reviewed the blog post
            var existingReview = _context.Reviews
                .FirstOrDefault(r => r.BlogPostId == reviewDto.BlogPostId && r.UserId == userId);

            if (existingReview != null)
            {
                // Update the existing review
                existingReview.Content = reviewDto.Content;
                existingReview.Rating = reviewDto.Rating;
                existingReview.CreatedAt = DateTime.UtcNow;

                _context.Reviews.Update(existingReview);
            }
            else
            {
                // Add a new review
                var newReview = new Review
                {
                    BlogPostId = reviewDto.BlogPostId,
                    UserId = userId,
                    Content = reviewDto.Content,
                    Rating = reviewDto.Rating,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Reviews.Add(newReview);
            }

            // Save changes to the database
            _context.SaveChanges();

            return Ok(new { Message = "Review added successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while adding the review.", Error = ex.Message });
        }
    }

    [HttpGet("list/{blogPostId}")]
    public IActionResult GetReviews(int blogPostId)
    {
        var reviews = _context.Reviews
            .Where(r => r.BlogPostId == blogPostId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Content,
                r.Rating,
                r.CreatedAt,
                User = new { r.User.Id, r.User.Username } // Include user information
            })
            .ToList();

        if (!reviews.Any())
        {
            return NotFound(new { Message = "No reviews found for this blog post." });
        }

        return Ok(reviews);
    }
}
