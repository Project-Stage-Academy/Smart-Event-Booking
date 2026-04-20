using SmartEventBooking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartEventBooking.Application.DTOs.CreateEvent
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDateTime { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDateTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int TotalCapacity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }

        public string? Banner { get; set; }

        [Required(ErrorMessage = "Status is required")]

        [Range(1, 4, ErrorMessage = "Please select a valid status")]
        public EventStatus? Status { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public Guid VenueId { get; set; }
    }
}
