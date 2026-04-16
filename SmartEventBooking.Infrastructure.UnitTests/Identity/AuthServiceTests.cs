using Moq;
using FluentAssertions;
using SmartEventBooking.Infrastructure.Identity;
using SmartEventBooking.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Shared.Constants;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Infrastructure.UnitTests.Helpers;

namespace SmartEventBooking.Infrastructure.UnitTests.Identity;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _signInManagerMock = MockHelpers.MockSignInManager(_userManagerMock);
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorAndNotCallIdentity_WhenPasswordsDoNotMatch()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "AnotherPassword123!",
            FirstName = "Ivan",
            LastName = "Ivanov"
        };

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Passwords mismatch.");

        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenIdentitySucceeds()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Ivan",
            LastName = "Ivanov"
        };

        ApplicationUser? createdUser = null;

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .Callback<ApplicationUser, string>((user, _) => createdUser = user)
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleConstants.User))
            .ReturnsAsync(IdentityResult.Success);

        User? addedDomainUser = null;
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => addedDomainUser = user);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeTrue();

        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be(dto.Email);
        createdUser.Id.Should().NotBe(Guid.Empty);

        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleConstants.User), Times.Once);

        addedDomainUser.Should().NotBeNull();
        addedDomainUser!.FirstName.Should().Be(dto.FirstName);
        addedDomainUser.LastName.Should().Be(dto.LastName);
        addedDomainUser.Id.Should().Be(createdUser.Id);

        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorsAndRollbackIdentityUser_WhenRoleAssignmentFails()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Ivan",
            LastName = "Ivanov"
        };

        var roleError = new IdentityError { Description = "Role assignment failed" };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleConstants.User))
            .ReturnsAsync(IdentityResult.Failed(roleError));

        _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Role assignment failed");

        _userManagerMock.Verify(x => x.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorsAndNotPersist_WhenIdentityFails()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Ivan",
            LastName = "Ivanov"
        };

        var identityError = new IdentityError { Description = "Password is too weak" };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Password is too weak");

        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenPasswordSignInSucceeds()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = true
        };

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnInvalidCredentials_WhenPasswordSignInFails()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "WrongPassword!",
            RememberMe = false
        };

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LogoutAsync_ShouldCallSignOutOnce()
    {
        _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

        await _authService.LogoutAsync();

        _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }
}