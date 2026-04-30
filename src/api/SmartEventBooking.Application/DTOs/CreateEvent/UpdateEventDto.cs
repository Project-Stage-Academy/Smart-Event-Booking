using SmartEventBooking.Application.DTOs;
using SmartEventBooking.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartEventBooking.Application.DTOs.UpdateEvent
{
    public class UpdateEventDto
    {
        [Required]
        public Guid Id { get; set; }

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

        public List<CategoryInputDto> Categories { get; set; } = new();

        [Required(ErrorMessage = "Status is required")]

        [Range(1, 4, ErrorMessage = "Please select a valid status")]
        public EventStatus? Status { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public Guid VenueId { get; set; }
    }
}
