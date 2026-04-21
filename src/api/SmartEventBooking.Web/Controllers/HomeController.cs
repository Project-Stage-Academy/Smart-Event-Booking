using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Web.Models;

namespace SmartEventBooking.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
   
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error occurred. RequestId: {RequestId}", requestId);
        return View(new ErrorViewModel { RequestId = null });
    }
}
