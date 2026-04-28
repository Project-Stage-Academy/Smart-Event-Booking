using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Web.Controllers;
using Xunit;

namespace SmartEventBooking.Web.Tests.Controllers;

public class ErrorsControllerTests
{
    private readonly ErrorsController _controller;

    public ErrorsControllerTests()
    {
        _controller = new ErrorsController();
    }

    [Theory]
    [InlineData(400, "Bad Request")]
    [InlineData(401, "Unauthorized")]
    [InlineData(403, "Forbidden")]
    [InlineData(404, "Not Found")]
    [InlineData(500, "An unexpected error occurred")]
    public void HandleStatusCode_ReturnsProblemDetails(int statusCode, string expectedTitle)
    {
        // Act
        var result = _controller.HandleStatusCode(statusCode);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        
        Assert.Equal(statusCode, objectResult.StatusCode);
        Assert.Equal(expectedTitle, problemDetails.Title);
    }
}
