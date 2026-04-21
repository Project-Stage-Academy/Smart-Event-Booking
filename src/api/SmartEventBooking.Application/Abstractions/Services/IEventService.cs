using SmartEventBooking.Application.DTOs.CreateEvent;
using SmartEventBooking.Application.DTOs.UpdateEvent;
using SmartEventBooking.Application.DTOs.Event;
using SmartEventBooking.Application.DTOs.Detail;
using SmartEventBooking.Application.DTOs.Venue;
using SmartEventBooking.Application.DTOs.Common;

namespace SmartEventBooking.Application.Abstractions.Services
{
    public interface IEventService
    {
        Task<PaginatedListDto<EventDto>> GetAllAsync(int page = 1, int pageSize = 5, CancellationToken cancellationToken = default);
        Task<PaginatedListDto<EventDto>> GetUpcomingAsync(int page = 1, int pageSize = 5, CancellationToken cancellationToken = default);
        Task<DetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid?> CreateAsync(CreateEventDto request, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(UpdateEventDto request, CancellationToken cancellationToken = default);
        Task<List<VenueDto>> GetVenuesAsync(CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}