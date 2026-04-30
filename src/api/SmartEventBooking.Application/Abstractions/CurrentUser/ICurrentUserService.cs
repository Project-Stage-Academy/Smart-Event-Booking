namespace SmartEventBooking.Application.Abstractions.CurrentUser;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
