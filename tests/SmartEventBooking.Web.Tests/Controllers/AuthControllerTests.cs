using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.DTOs.Auth;
using SmartEventBooking.Web.Controllers;
using System.Security.Claims;

namespace SmartEventBooking.Web.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Ivan"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(dto))
            .ReturnsAsync(new AuthResultDto { Succeeded = true });

        var result = await _controller.Register(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
    {
        var dto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Ivan"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(dto))
            .ReturnsAsync(new AuthResultDto { Succeeded = false, Errors = ["Email already taken"] });

        var result = await _controller.Register(dto);

        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        (badRequestResult?.Value as IEnumerable<string>).Should().Contain("Email already taken");
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenLoginSucceeds()
    {
        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            RememberMe = true
        };

        _authServiceMock.Setup(x => x.LoginAsync(dto))
            .ReturnsAsync(new AuthResultDto { Succeeded = true });

        var result = await _controller.Login(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenLoginFails()
    {
        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            RememberMe = true
        };

        _authServiceMock.Setup(x => x.LoginAsync(dto))
            .ReturnsAsync(new AuthResultDto { Succeeded = false, Errors = ["Invalid credentials"] });

        var result = await _controller.Login(dto);

        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        (badRequestResult?.Value as IEnumerable<string>).Should().Contain("Invalid credentials");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Login_ForwardsRememberMeToAuthService(bool rememberMe)
    {
        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            RememberMe = rememberMe
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(new AuthResultDto { Succeeded = true });

        await _controller.Login(dto);

        _authServiceMock.Verify(
            x => x.LoginAsync(It.Is<LoginDto>(d =>
                d.Email == dto.Email &&
                d.Password == dto.Password &&
                d.RememberMe == rememberMe)),
            Times.Once);
    }
}
