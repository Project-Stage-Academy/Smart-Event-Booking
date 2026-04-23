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
        public async Task Register_Post_RedirectsToHome_WhenRegistrationSucceeds()
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
            redirectResult?.ActionName.Should().Be("Index");
            redirectResult?.ControllerName.Should().Be("Home");
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
            _controller.ModelState.Values.SelectMany(v => v.Errors)
                .Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Error 1");
        }

        [Fact]
        public async Task Login_ReturnsViewResult_WhenNotAuthenticated()
        {
            var result = _controller.Login();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Email", "Required");
            var dto = new LoginDto 
            { 
                Email = "", 
                Password = "123",
                RememberMe = true 
            };

            var result = await _controller.Login(dto);

            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult?.Model.Should().Be(dto);
        }

        [Fact]
        public async Task Login_Post_RedirectsToHome_WhenRegistrationSucceeds()
        {
            var dto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                RememberMe = true
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(new AuthResultDto { Succeeded = true });

            var result = await _controller.Login(dto);

            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult?.ActionName.Should().Be("Index");
            redirectResult?.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_Post_ReturnsViewWithErrors_WhenLoginFails()
        {
            var dto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password123!",
                RememberMe = true
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(new AuthResultDto { Succeeded = false, Errors = new[] { "Error 1" } });

            var result = await _controller.Login(dto);

            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.Values.SelectMany(v => v.Errors)
                .Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Error 1");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Login_Post_ForwardsRememberMeToAuthService(bool rememberMe)
        {
            var dto = new LoginDto
            {
                Email = "test@test.com",
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
}
