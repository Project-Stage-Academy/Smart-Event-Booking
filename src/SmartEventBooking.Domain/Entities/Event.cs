using SmartEventBooking.Domain.Enums;
using System;

namespace SmartEventBooking.Domain.Entities;

public class Event
{
    public Guid Id { get; init; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Banner { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public int TotalCapacity { get; private set; }
    public int AvailableSeats { get; private set; }
    public decimal Price { get; private set; }
    public EventStatus Status { get; private set; }
    public byte[]? RowVersion { get; private set; }
    public Guid VenueId { get; private set; }
    public Venue Venue { get; private set; } = null!;

    private Event() { }

    public Event(Guid id, string title, string? description, DateTime startDateTime, DateTime endDateTime, int totalCapacity, decimal price, Guid venueId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (totalCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(totalCapacity));
        if (startDateTime >= endDateTime)
            throw new ArgumentException("Start date must be before end date.", nameof(startDateTime));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Id = id;
        Title = title;
        Description = description;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        TotalCapacity = totalCapacity;
        AvailableSeats = totalCapacity;
        Price = price;
        Status = EventStatus.Active; 
        VenueId = venueId;
    }

    public void BookSeats(int seatsToBook)
    {
        if (seatsToBook <= 0)
            throw new ArgumentException("Must book at least one seat.", nameof(seatsToBook));
            
        if (AvailableSeats < seatsToBook)
            throw new InvalidOperationException("Not enough available seats.");

        AvailableSeats -= seatsToBook;
    }

    public void CancelBooking(int seatsToCancel)
    {
        if (seatsToCancel <= 0)
            throw new ArgumentException("Must cancel at least one seat.", nameof(seatsToCancel));
            
        if (AvailableSeats + seatsToCancel > TotalCapacity)
            throw new InvalidOperationException("Cannot exceed total capacity.");

        AvailableSeats += seatsToCancel;
    }
    
    public void SetBanner(string bannerPath)
    {
        Banner = bannerPath;
    }

    public void UpdateCapacity(int totalCapacity, int availableSeats)
    {
        if (availableSeats > totalCapacity)
            throw new InvalidOperationException("Available seats cannot exceed total capacity");
            
        TotalCapacity = totalCapacity;
        AvailableSeats = availableSeats;
    }
}
