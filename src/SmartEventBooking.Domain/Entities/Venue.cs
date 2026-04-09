using System;

namespace SmartEventBooking.Domain.Entities;

public class Venue
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Venue() { }

    public Venue(Guid id, string name, string location, int capacity, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

        Id = id;
        Name = name;
        Location = location;
        Capacity = capacity;
        CreatedAt = createdAt;
    }
}
