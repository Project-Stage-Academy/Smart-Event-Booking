using Microsoft.EntityFrameworkCore;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Application.DTOs.Event;

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

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Event>> GetUpcomingAsync(int skip, int take, EventSearchDto? searchDto = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Events.Where(e => e.StartDateTime >= DateTime.UtcNow);

        if (searchDto != null)
        {
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(keyword) || (e.Description != null && e.Description.ToLower().Contains(keyword)));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(e => e.EventCategories.Any(ec => ec.CategoryId == searchDto.CategoryId.Value));
            }
            if (searchDto.StartDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= searchDto.StartDate.Value);
            }

            if (searchDto.EndDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= searchDto.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Location))
            {
                var loc = searchDto.Location.ToLower();
                query = query.Where(e => e.Venue.Name.ToLower().Contains(loc) || (e.Venue.Location != null && e.Venue.Location.ToLower().Contains(loc)));
            }
        }

        return await query
            .OrderBy(e => e.StartDateTime)
            .Skip(skip)
            .Take(take)
            .Include(e => e.Venue)
            .Include(e => e.EventCategories)
                .ThenInclude(ec => ec.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUpcomingCountAsync(EventSearchDto? searchDto = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Events.Where(e => e.StartDateTime >= DateTime.UtcNow);

        if (searchDto != null)
        {
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(keyword) || (e.Description != null && e.Description.ToLower().Contains(keyword)));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(e => e.EventCategories.Any(ec => ec.CategoryId == searchDto.CategoryId.Value));
            }
            if (searchDto.StartDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= searchDto.StartDate.Value);
            }

            if (searchDto.EndDate.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= searchDto.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Location))
            {
                var loc = searchDto.Location.ToLower();
                query = query.Where(e => e.Venue.Name.ToLower().Contains(loc) || (e.Venue.Location != null && e.Venue.Location.ToLower().Contains(loc)));
            }
        }

        return await query.CountAsync(cancellationToken);
    }

    public void Add(Event @event)
    {
        _context.Events.Add(@event);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }

    public void Delete(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Events.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
