
namespace SmartEventBooking.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
