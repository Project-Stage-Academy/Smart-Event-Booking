
using SmartEventBooking.Application.DTOs.Auth;

namespace SmartEventBooking.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task LogoutAsync();
    }
}
