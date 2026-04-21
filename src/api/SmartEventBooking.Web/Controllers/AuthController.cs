using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.DTOs.Auth;

namespace SmartEventBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result.Succeeded)
        {
            return Ok(new { message = "Registration successful" });
        }

        return BadRequest(result.Errors);
    }
}