namespace SmartEventBooking.Application.DTOs.Event;

public class EventSearchDto
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
}