
namespace SmartEventBooking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
