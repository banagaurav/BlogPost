using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationController(AppDbContext context)
    {
        _context = context;
    }

    // Get all notifications for the logged-in user
    [HttpGet]
    public IActionResult GetNotifications()
    {
        var userId = int.Parse(User.FindFirst("id").Value); // Assuming JWT contains 'id'
        var notifications = _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();

        return Ok(notifications);
    }

    // Mark a notification as read
    [HttpPost("mark-as-read")]
    public IActionResult MarkAsRead([FromBody] int notificationId)
    {
        var userId = int.Parse(User.FindFirst("id").Value); // Assuming JWT contains 'id'
        var notification = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
        if (notification == null)
        {
            return NotFound(new { Message = "Notification not found" });
        }

        notification.IsRead = true;
        _context.SaveChanges();

        return Ok(new { Message = "Notification marked as read" });
    }

    // Delete a notification
    [HttpDelete("{id}")]
    public IActionResult DeleteNotification(int id)
    {
        var userId = int.Parse(User.FindFirst("id").Value); // Assuming JWT contains 'id'
        var notification = _context.Notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
        if (notification == null)
        {
            return NotFound(new { Message = "Notification not found" });
        }

        _context.Notifications.Remove(notification);
        _context.SaveChanges();

        return Ok(new { Message = "Notification deleted" });
    }
}
