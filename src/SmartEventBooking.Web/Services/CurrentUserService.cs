using System.Security.Claims;
using SmartEventBooking.Application.Abstractions.CurrentUser;

namespace SmartEventBooking.Web.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var identity = user?.Identity;
            return identity?.IsAuthenticated == true;
        }
    }
}
