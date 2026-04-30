using System;
using System.Collections.Generic;
using System.Text;
using SmartEventBooking.Application.DTOs.Venue;

namespace SmartEventBooking.Application.DTOs.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal Price { get; set; }
        public VenueDto Venue { get; set; }
    }
}
