using System.Security.Claims;
using SmartEventBooking.Application.Abstractions.CurrentUser;

namespace SmartEventBooking.Web.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
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
