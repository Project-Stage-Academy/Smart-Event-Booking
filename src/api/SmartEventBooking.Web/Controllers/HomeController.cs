using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartEventBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { message = "Welcome to Smart Event Booking API" });
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return Ok(new { message = "Privacy Policy Content" });
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error occurred. RequestId: {RequestId}", requestId);

        return Problem(
            detail: "An unexpected error occurred processing your request.",
            title: "Internal Server Error",
            instance: requestId
        );
    }
}