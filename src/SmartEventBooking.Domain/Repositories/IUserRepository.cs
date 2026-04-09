using SmartEventBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEventBooking.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}
