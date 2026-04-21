using SmartEventBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    private readonly List<EventCategory> _eventCategories = new();
    public IReadOnlyCollection<EventCategory> EventCategories => _eventCategories.AsReadOnly();

    private Event() { }

    public Event(Guid id, string title, string? description, DateTime startDateTime, DateTime endDateTime, int totalCapacity, decimal price, Guid venueId, EventStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));
        if (totalCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(totalCapacity));
        if (startDateTime < DateTime.Now)
            throw new ArgumentException("Start date cannot be in the past.", nameof(startDateTime));
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
        Status = status; 
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
        if (totalCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(totalCapacity));
        if (availableSeats < 0)
            throw new ArgumentException("Available seats cannot be negative.", nameof(availableSeats));
        if (availableSeats > totalCapacity)
            throw new InvalidOperationException("Available seats cannot exceed total capacity.");
  
            
        TotalCapacity = totalCapacity;
        AvailableSeats = availableSeats;
    }

    public void Update(string title, string? description, string? banner,  DateTime startDateTime, DateTime endDateTime, int totalCapacity, decimal price,  Guid venueId, EventStatus status)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (totalCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(totalCapacity));

        if (startDateTime < DateTime.Now)
            throw new ArgumentException("Start date cannot be in the past.", nameof(startDateTime));

        if (startDateTime >= endDateTime)
            throw new ArgumentException("Start date must be before end date.", nameof(startDateTime));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Title = title;
        Description = description;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;

        var bookedSeats = TotalCapacity - AvailableSeats;
        var newAvailableSeats = totalCapacity - bookedSeats;

       
        UpdateCapacity(totalCapacity, newAvailableSeats);

        Price = price;
        VenueId = venueId;
        Banner = banner;
        Status = status;
    }

    public void AddCategory(EventCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        if (_eventCategories.Any(c => c.Id == category.Id))
            throw new InvalidOperationException("This category is already added to the event.");

        _eventCategories.Add(category);
    }

    public void RemoveCategory(EventCategory category)
    {
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        var existingCategory = _eventCategories.FirstOrDefault(c => c.Id == category.Id);
        if (existingCategory == null)
            throw new InvalidOperationException("Category not found in this event.");

        _eventCategories.Remove(existingCategory);
    }

    public void ClearCategories()
    {
        _eventCategories.Clear();
    }
}
