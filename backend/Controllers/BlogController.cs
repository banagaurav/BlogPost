using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // Create a new blog (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateBlog([FromBody] Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set metadata for the blog
            blog.CreatedAt = DateTime.UtcNow;
            blog.Author = User.Identity.Name; // The username of the logged-in admin

            _context.Blogs.Add(blog);
            _context.SaveChanges();

            // Notify all users
            var users = _context.Users.ToList();
            var notifications = users.Select(user => new Notification
            {
                UserId = user.Id,
                Message = $"A new blog titled '{blog.Title}' has been posted!",
            }).ToList();

            _context.Notifications.AddRange(notifications);
            _context.SaveChanges();

            return Ok(new { Message = "Blog created successfully.", BlogId = blog.Id });
        }

        // Get all blogs (accessible by everyone)
        [HttpGet]
        public IActionResult GetAllBlogs()
        {
            var blogs = _context.Blogs.ToList();
            return Ok(blogs);
        }

        // Get a blog by ID (accessible by everyone)
        [HttpGet("{id}")]
        public IActionResult GetBlogById(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
            {
                return NotFound(new { Message = "Blog not found." });
            }
            return Ok(blog);
        }
    }
}
