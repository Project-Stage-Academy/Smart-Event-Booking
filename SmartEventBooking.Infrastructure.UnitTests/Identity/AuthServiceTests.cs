using Moq;
using FluentAssertions;
using SmartEventBooking.Infrastructure.Identity;
using SmartEventBooking.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Shared.Constants;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Infrastructure.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SmartEventBooking.Infrastructure.UnitTests.Identity;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _signInManagerMock = MockHelpers.MockSignInManager(_userManagerMock);
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _unitOfWorkMock
            .Setup(x => x.ExecuteWithStrategyAsync(It.IsAny<Func<Task<AuthResultDto>>>(), default))
            .Returns((Func<Task<AuthResultDto>> operation, CancellationToken _) => operation());

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccessAndSignIn_WhenIdentitySucceeds()
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

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync(default)).Returns(Task.CompletedTask);

        _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

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
        _unitOfWorkMock.Verify(x => x.ExecuteWithStrategyAsync(It.IsAny<Func<Task<AuthResultDto>>>(), default), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(default), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(default), Times.Once);
        _signInManagerMock.Verify(x => x.SignInAsync(createdUser, false, null), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorsAndRollback_WhenRoleAssignmentFails()
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

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.RollbackTransactionAsync(default)).Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Role assignment failed");

        _unitOfWorkMock.Verify(x => x.ExecuteWithStrategyAsync(It.IsAny<Func<Task<AuthResultDto>>>(), default), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(default), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(default), Times.Once);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(default), Times.Never);
        _signInManagerMock.Verify(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnErrorsAndRollback_WhenIdentityFails()
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

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.RollbackTransactionAsync(default)).Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Password is too weak");

        _unitOfWorkMock.Verify(x => x.ExecuteWithStrategyAsync(It.IsAny<Func<Task<AuthResultDto>>>(), default), Times.Once);
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(default), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(default), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(default), Times.Never);
        _signInManagerMock.Verify(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = true
        };

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(appUser);

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, true))
            .ReturnsAsync(SignInResult.Success);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeTrue();
        _userManagerMock.Verify(x => x.FindByEmailAsync(dto.Email), Times.Once);
        _signInManagerMock.Verify(x => x.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, true), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnErrors_WhenUserDoesNotExist()
    {
        var dto = new LoginDto
        {
            Email = "missing@test.com",
            Password = "Password123!",
            RememberMe = false
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Login failed.");
        _signInManagerMock.Verify(x => x.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnErrors_WhenUserIsLockedOut()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = true
        };

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(appUser);

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, true))
            .ReturnsAsync(SignInResult.LockedOut);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Login failed.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnErrors_WhenUserIsNotAllowed()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = true
        };

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(appUser);

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, true))
            .ReturnsAsync(SignInResult.NotAllowed);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Login failed.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnErrors_WhenPasswordIsInvalid()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "wrong-password",
            RememberMe = false
        };

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(appUser);

        _signInManagerMock.Setup(x => x.PasswordSignInAsync(appUser, dto.Password, dto.RememberMe, true))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("Login failed.");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task LoginAsync_ShouldPassRememberMeToPasswordSignIn(bool rememberMe)
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = rememberMe
        };

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(appUser);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(appUser, dto.Password, rememberMe, true))
            .ReturnsAsync(SignInResult.Success);

        var result = await _authService.LoginAsync(dto);

        result.Succeeded.Should().BeTrue();
        _signInManagerMock.Verify(
            x => x.PasswordSignInAsync(appUser, dto.Password, rememberMe, true),
            Times.Once);
    }
}
