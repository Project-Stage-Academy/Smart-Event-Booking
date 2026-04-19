using System.ComponentModel.DataAnnotations;

namespace SmartEventBooking.Domain.Enums;

public enum EventStatus : byte
{
    [Display(Name = "Активний")]
    Active = 1,

    [Display(Name = "Скасований")]
    Cancelled = 2,

    [Display(Name = "Завершений")]
    Completed = 3,

    [Display(Name = "Продано")]
    SoldOut = 4
}
