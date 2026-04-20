using Microsoft.EntityFrameworkCore;
using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Infrastructure.Persistence; 

namespace SmartEventBooking.Infrastructure.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _context;

        public VenueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Venues.ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Venues.AnyAsync(v => v.Id == id, cancellationToken);
        }
    }
}