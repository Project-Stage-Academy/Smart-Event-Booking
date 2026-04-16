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
        public void Register_ReturnsViewResult_WhenNotAuthenticated()
        {
            var result = _controller.Register();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Register_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Email", "Required");
            var dto = new RegisterDto 
            { 
                Email = "", 
                Password = "123", 
                ConfirmPassword = "123", 
                FirstName = "Test" 
            };

            var result = await _controller.Register(dto);

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult?.Model.Should().Be(dto);
        }

        [Fact]
        public async Task Register_Post_RedirectsToLogin_WhenRegistrationSucceeds()
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

            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult?.ActionName.Should().Be("Login");
        }

        [Fact]
        public async Task Register_Post_ReturnsViewWithErrors_WhenRegistrationFails()
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

            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState[string.Empty]?.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("Error 1");
        }
    }
}
