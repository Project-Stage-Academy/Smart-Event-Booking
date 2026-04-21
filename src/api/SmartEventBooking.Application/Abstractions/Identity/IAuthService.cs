
using SmartEventBooking.Application.DTOs.Auth;

namespace SmartEventBooking.Application.Abstractions.Identity
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
    }
}
