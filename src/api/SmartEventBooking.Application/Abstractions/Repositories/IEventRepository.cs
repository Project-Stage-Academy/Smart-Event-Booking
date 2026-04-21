using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Application.DTOs.Event;

namespace SmartEventBooking.Application.Abstractions.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetUpcomingAsync(int skip, int take, EventSearchDto? searchDto = null, CancellationToken cancellationToken = default);
    Task<int> GetUpcomingCountAsync(EventSearchDto? searchDto = null, CancellationToken cancellationToken = default);
    void Add(Event @event);
    void Update(Event @event);
    void Delete(Event @event);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
}
