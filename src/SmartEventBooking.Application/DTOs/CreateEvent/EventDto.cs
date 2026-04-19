using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEventBooking.Application.DTOs.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal Price { get; set; }
    }
}
