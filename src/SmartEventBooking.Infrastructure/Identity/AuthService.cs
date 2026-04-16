using Microsoft.AspNetCore.Identity;
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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return new AuthResultDto 
                { 
                    Succeeded = false, 
                    Errors = new[] { "Passwords mismatch." } 
                };
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var appUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = dto.Email,
                    Email = dto.Email
                };

                var result = await _userManager.CreateAsync(appUser, dto.Password);

                if (!result.Succeeded)
                {
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
                
                // Sign-in after successful registration and commit
                await _signInManager.SignInAsync(appUser, isPersistent: false);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return new AuthResultDto { Succeeded = true };
        }
    }
}
