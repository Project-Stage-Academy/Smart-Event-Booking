
namespace SmartEventBooking.Shared.Configuration
{
    public sealed class AdminOptions
    {
        public const string SectionName = "AdminSettings";
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
