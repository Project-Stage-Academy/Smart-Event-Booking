namespace SmartEventBooking.Application.Abstractions.CurrentUser;

public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}
