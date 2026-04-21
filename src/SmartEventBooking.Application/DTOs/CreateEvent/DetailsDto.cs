using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using SmartEventBooking.Domain.Enums;
using System.Text;

namespace SmartEventBooking.Application.DTOs.Detail
{
    public class DetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Banner { get; set; } 
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int TotalCapacity { get; set; }
        public int AvailableSeats { get; set; } 
        public decimal Price { get; set; }
        public EventStatus Status { get; set; }
        public Guid VenueId { get; set; }
    }


}
