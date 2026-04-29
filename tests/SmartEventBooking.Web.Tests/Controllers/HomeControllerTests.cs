using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using SmartEventBooking.Web.Controllers;

namespace SmartEventBooking.Web.Tests.Controllers;

public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsOkResult()
    {
        var controller = new HomeController(NullLogger<HomeController>.Instance);

        var result = controller.Index();

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Privacy_ReturnsOkResult()
    {
        var controller = new HomeController(NullLogger<HomeController>.Instance);

        var result = controller.Privacy();

        result.Should().BeOfType<OkObjectResult>();
    }
}
