using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.DTOs.Auth;
using SmartEventBooking.Web.Controllers;

namespace SmartEventBooking.Web.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();

    private AuthController CreateController() => new(_authServiceMock.Object);

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

        var result = await CreateController().Register(dto);

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

        var result = await CreateController().Register(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
