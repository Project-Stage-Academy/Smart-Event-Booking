using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartEventBooking.Application.Abstractions.Identity;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Application.DTOs.Auth;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Shared.Constants;

namespace SmartEventBooking.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Starting registration for user {Email}", dto.Email);

            ApplicationUser? createdUser = null;

            var result = await _unitOfWork.ExecuteWithStrategyAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var appUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = dto.Email,
                        Email = dto.Email
                    };

                    createdUser = appUser;

                    var result = await _userManager.CreateAsync(appUser, dto.Password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogWarning("Identity user creation failed for {Email}: {Errors}", dto.Email, errors);

                        await _unitOfWork.RollbackTransactionAsync();
                        return new AuthResultDto
                        {
                            Succeeded = false,
                            Errors = result.Errors.Select(e => e.Description)
                        };
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(appUser, RoleConstants.User);
                    if (!addToRoleResult.Succeeded)
                    {
                        var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                        _logger.LogWarning("Adding user {Email} to role failed: {Errors}", dto.Email, errors);

                        await _unitOfWork.RollbackTransactionAsync();
                        return new AuthResultDto
                        {
                            Succeeded = false,
                            Errors = addToRoleResult.Errors.Select(e => e.Description)
                        };
                    }

                    var domainUser = new User
                    {
                        Id = appUser.Id,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    };
                    _userRepository.Add(domainUser);

                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation("User {Email} registered successfully", dto.Email);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "An error occurred during registration for user {Email}", dto.Email);
                    throw;
                }

                return new AuthResultDto 
                { 
                    Succeeded = true,
                    Roles = new[] { RoleConstants.User }
                };
            });

            if (result.Succeeded && createdUser is not null)
            {
                await _signInManager.SignInAsync(createdUser, isPersistent: false);
            }

            return result;
        }

        protected async Task<AuthResultDto> HandleLogin(ApplicationUser appUser, LoginDto dto)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(
                appUser, 
                dto.Password, 
                dto.RememberMe, 
                lockoutOnFailure: true
            );
            if (loginResult.IsLockedOut)
            {
                _logger.LogInformation("Login failed for user {Email}: User locked out", dto.Email);
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new[] { "Login failed: Rate limited." }
                };
            }
            else if (loginResult.IsNotAllowed)
            {
                _logger.LogInformation("Login failed for user {Email}: User not allowed to sign in", dto.Email);
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new[] { "Login failed: Please verify your email (should never happen for now)." } // TODO if email verification is implemented, reword
                };
            }
            if (!loginResult.Succeeded)
            {
                _logger.LogInformation("Login failed for user {Email}: Invalid password", dto.Email);
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new[] { "Login failed." }
                };
            }

            var roles = await _userManager.GetRolesAsync(appUser);
            return new AuthResultDto
            {
                Succeeded = true,
                Roles = roles
            };
        } 

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Starting login for user {Email}", dto.Email);

            ApplicationUser? appUser = null;

            AuthResultDto? result = null;

            try
            {
                appUser = await _userManager.FindByEmailAsync(dto.Email);
                if (appUser is null)
                {
                    _logger.LogInformation("Login failed for user {Email}: User does not exist", dto.Email);
                    return new AuthResultDto 
                    { 
                        Succeeded = false,
                        Errors = new[] { "Login failed." }
                    };
                }

                result = await HandleLogin(appUser, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user {Email}", dto.Email);
                throw;
            }

            return result;
        }
    }
}
