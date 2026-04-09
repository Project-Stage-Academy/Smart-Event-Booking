using SmartEventBooking.Domain.Entities;
using SmartEventBooking.Domain.Repositories;
using SmartEventBooking.Infrastructure.Persistence;

namespace SmartEventBooking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.DomainUsers.FindAsync(new object[] { id }, cancellationToken);
        }
        public void Add(User user)
        {
            _dbContext.DomainUsers.Add(user);
        }

        public void Update(User user)
        {
            _dbContext.DomainUsers.Update(user);
        }

        public void Delete(User user)
        {
            _dbContext.DomainUsers.Remove(user);
        }

    }
}
