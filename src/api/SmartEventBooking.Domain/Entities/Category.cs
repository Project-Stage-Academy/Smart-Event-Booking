using System;
using System.Collections.Generic;

namespace SmartEventBooking.Domain.Entities;

public class Category
{
    public int Id { get; init; }
    public string Name { get; private set; } = string.Empty;

    private readonly List<EventCategory> _eventCategories = new();
    public IReadOnlyCollection<EventCategory> EventCategories => _eventCategories.AsReadOnly();

    private Category() { }

    public Category(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Id = id;
        Name = name;
    }
}