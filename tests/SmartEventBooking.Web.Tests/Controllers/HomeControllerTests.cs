using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartEventBooking.Web.Controllers;
using SmartEventBooking.Web.Models;

namespace SmartEventBooking.Web.Tests.Controllers;

public class HomeControllerTests
{
    [Fact]
    public void Index_ReturnsViewResult()
    {
        var controller = new HomeController();

        var result = controller.Index();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        var controller = new HomeController();

        var result = controller.Privacy();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ReturnsViewWithErrorViewModel()
    {
        var controller = new HomeController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = controller.Error();

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeOfType<ErrorViewModel>();
    }
}
