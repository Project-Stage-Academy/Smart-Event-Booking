using System;

namespace SmartEventBooking.Domain.Entities;

public class EventCategory
{
    public Guid Id { get; init; }
    public int CategoryId { get; private set; }
    public Guid EventId { get; private set; }

    public Category Category { get; private set; } = null!;
    public Event Event { get; private set; } = null!;

    private EventCategory() { }

    public EventCategory(Guid id, int categoryId, Guid eventId)
    {
        Id = id;
        CategoryId = categoryId;
        EventId = eventId;
    }
}