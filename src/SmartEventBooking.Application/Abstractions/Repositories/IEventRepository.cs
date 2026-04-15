using SmartEventBooking.Domain.Entities;
namespace SmartEventBooking.Application.Abstractions.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default);
    void Add(Event @event);
    void Update(Event @event);
    void Delete(Event @event);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
