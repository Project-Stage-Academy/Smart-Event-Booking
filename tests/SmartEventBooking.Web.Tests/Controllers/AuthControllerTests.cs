using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using SmartEventBooking.Web.Controllers;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SmartEventBooking.Web.Tests.Controllers
{
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
                Email = "test@test.com",
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
                Email = "test@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Ivan"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(new AuthResultDto { Succeeded = false, Errors = new[] { "Error 1" } });

            var result = await _controller.Register(dto);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            (badRequestResult?.Value as IEnumerable<string>).Should().Contain("Error 1");
        }
    }
}
