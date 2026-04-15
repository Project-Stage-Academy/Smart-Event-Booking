namespace SmartEventBooking.Domain.Enums;

public enum EventStatus : byte
{
    Active = 1,
    Cancelled = 2,
    Completed = 3,
    SoldOut = 4
}
