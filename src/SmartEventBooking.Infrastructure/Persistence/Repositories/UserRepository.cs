using SmartEventBooking.Application.Abstractions.Repositories;
using SmartEventBooking.Domain.Entities;

namespace SmartEventBooking.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DomainUsers.FindAsync(new object[] { id }, cancellationToken);
    }
    public void Add(User user)
    {
        _context.DomainUsers.Add(user);
    }

    public void Update(User user)
    {
        _context.DomainUsers.Update(user);
    }

    public void Delete(User user)
    {
        _context.DomainUsers.Remove(user);
    }

}

