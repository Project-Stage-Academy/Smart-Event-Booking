using SmartEventBooking.Domain.Enums;
using System;

namespace SmartEventBooking.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Banner { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int TotalCapacity { get; set; }
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
    public EventStatus Status { get; set; }
    public byte[]? RowVersion { get; set; }
    public Guid VenueId { get; set; }
}
