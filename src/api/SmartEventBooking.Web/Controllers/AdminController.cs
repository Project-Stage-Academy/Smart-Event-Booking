using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Shared.Constants;

namespace SmartEventBooking.Web.Controllers
{
    /// <summary>
    /// Контролер для адміністрування - доступний тільки адміністраторам
    /// </summary>
    [Authorize(Roles = RoleConstants.Admin)]  // ← Потрібна Admin роль
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// Отримати статистику - тільки для адміністраторів
        /// </summary>
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

        /// <summary>
        /// Отримати користувачів - тільки для адміністраторів
        /// </summary>
        [HttpGet("users")]
        public IActionResult GetUsers([FromQuery] int page = 1)
        {
            return Ok(new 
            { 
                page = page,
                totalUsers = 10,
                users = new List<object>
                {
                    new { id = 1, email = "user@example.com", role = "User" },
                    new { id = 2, email = "admin@example.com", role = "Admin" }
                }
            });
        }

        /// <summary>
        /// Видалити користувача - тільки для адміністраторів
        /// </summary>
        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            return NoContent();  // 204 No Content
        }
    }
}