using Microsoft.EntityFrameworkCore;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Domain.Entities;

namespace SmartEventBooking.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Events.OrderBy(e => e.Id).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public void Add(Event @event)
    {
        _context.Events.Add(@event);
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }

    public void Delete(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
