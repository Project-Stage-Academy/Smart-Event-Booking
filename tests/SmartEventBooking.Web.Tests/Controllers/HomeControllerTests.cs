using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using SmartEventBooking.Web.Controllers;

namespace SmartEventBooking.Web.Tests.Controllers;

public class HomeControllerTests
{
    private static HomeController CreateController() =>
        new(NullLogger<HomeController>.Instance)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

    [Fact]
    public void Index_ReturnsOkResult()
    {
        var result = CreateController().Index();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Privacy_ReturnsOkResult()
    {
        var result = CreateController().Privacy();

        result.Should().BeOfType<OkObjectResult>();
    }
}