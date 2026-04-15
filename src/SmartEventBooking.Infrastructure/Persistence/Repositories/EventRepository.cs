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
        return await _context.Set<Event>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Event>().OrderBy(e => e.Id).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Set<Event>().AddAsync(@event, cancellationToken);
    }

    public Task UpdateAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Set<Event>().Update(@event);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Event @event, CancellationToken cancellationToken = default)
    {
        _context.Set<Event>().Remove(@event);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Event>().AnyAsync(e => e.Id == id, cancellationToken);
    }
}
