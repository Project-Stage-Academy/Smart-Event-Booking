using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Web.Models;

namespace SmartEventBooking.Web.Controllers;

public class ErrorsController : Controller
{
    [Route("Errors/{statusCode}")]
    public IActionResult HandleStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => View("BadRequest"),
            403 => View("Forbidden"),
            404 => View("NotFound"),
            _ => View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier })
        };
    }

    [Route("Errors/Error")]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
