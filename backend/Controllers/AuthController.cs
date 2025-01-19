
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] string fullName, [FromForm] string username, [FromForm] string password, [FromForm] string gmail)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(gmail))
            {
                return BadRequest(new { Message = "All fields are required." });
            }

            // Check if username or Gmail already exists
            if (_context.Users.Any(u => u.Username == username || u.Gmail == gmail))
            {
                return Conflict(new { Message = "Username or Gmail already exists." });
            }

            // Create a new user object
            var user = new User
            {
                FullName = fullName,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password), // Hash the password
                Gmail = gmail,
                Role = "User" // Default role
            };

            // Save the user to the database
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Message = "Login successful.",
                Token = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
