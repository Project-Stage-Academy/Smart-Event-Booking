using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Shared.Constants;

namespace SmartEventBooking.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]  // ← Потрібна Admin роль
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            return Ok(new 
            { 
                totalUsers = 10,
                totalEvents = 25,
                totalBookings = 150,
                message = "Адміністративна статистика"
            });
        }

        [HttpGet("users")]
        public IActionResult GetUsers([FromQuery] int page = 1)
        {
            return Ok(new 
            { 
                page = page,
                totalUsers = 10,
                users = new List<object>
                {
                    new { id = 1, email = "user@example.com", role = RoleConstants.User },
                    new { id = 2, email = "admin@example.com", role = RoleConstants.Admin }
                }
            });
        }

        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            return NoContent();
        }
    }
}