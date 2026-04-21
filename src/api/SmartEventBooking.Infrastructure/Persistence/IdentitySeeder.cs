using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartEventBooking.Application.Abstractions.Data;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Infrastructure.Identity;
using SmartEventBooking.Shared.Configuration;
using SmartEventBooking.Shared.Constants;

namespace SmartEventBooking.Infrastructure.Persistence
{
    public sealed class IdentitySeeder : IDatabaseSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AdminOptions _adminOptions;

        public IdentitySeeder(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IOptions<AdminOptions> adminOptions)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _adminOptions = adminOptions.Value;
        }

        public async Task SeedAsync()
        {
            string[] roles = { RoleConstants.Admin, RoleConstants.User };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    if (!createRoleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role '{role}': {string.Join(", ", createRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            string adminEmail = _adminOptions.Email;
            string adminPassword = _adminOptions.Password;

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminId = Guid.NewGuid();

                var appAdmin = new ApplicationUser
                {
                    Id = adminId,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(appAdmin, adminPassword);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user '{adminEmail}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(appAdmin, RoleConstants.Admin);
                if (!addToRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(appAdmin);
                    throw new InvalidOperationException(
                        $"Failed to assign admin role to '{adminEmail}': {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }

                var domainAdmin = new User
                {
                    Id = adminId,
                    FirstName = "System",
                    LastName = "Administrator"
                };

                try
                {
                    _userRepository.Add(domainAdmin);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    var deleteAdminResult = await _userManager.DeleteAsync(appAdmin);
                    if (!deleteAdminResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to persist domain admin user for '{adminEmail}', and cleanup of the Identity user also failed: {string.Join(", ", deleteAdminResult.Errors.Select(e => e.Description))}",
                            ex);
                    }
                    throw new InvalidOperationException(
                        $"Failed to persist domain admin user for '{adminEmail}'. The created Identity user was deleted to avoid a partially-seeded state.",
                        ex);
                }
            }
        }
    }
}
