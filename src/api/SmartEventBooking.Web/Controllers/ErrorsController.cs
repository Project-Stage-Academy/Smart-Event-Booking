using Microsoft.AspNetCore.Mvc;

namespace SmartEventBooking.Web.Controllers;

/// <summary>
/// Controller for handling HTTP errors and returning RFC 7807 problem details.
/// </summary>
[ApiController]
public class ErrorsController : ControllerBase
{
    /// <summary>
    /// Handles the specified HTTP status code and returns a problem detail response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to handle.</param>
    /// <returns>An <see cref="IActionResult"/> with RFC 7807 problem details.</returns>
    [Route("Errors/{statusCode}")]
    [HttpGet]
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
