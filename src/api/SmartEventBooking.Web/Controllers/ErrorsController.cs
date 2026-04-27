using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartEventBooking.Web.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("Errors/{statusCode}")]
    public IActionResult HandleStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => Problem(title: "Bad Request", statusCode: 400),
            401 => Problem(title: "Unauthorized", statusCode: 401),
            403 => Problem(title: "Forbidden", statusCode: 403),
            404 => Problem(title: "Not Found", statusCode: 404),
            _ => Problem(title: "An unexpected error occurred", statusCode: statusCode)
        };
    }
}
