using SmartEventBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEventBooking.Application.Abstractions.Repositories
{
    public interface IVenueRepository
    {
        Task<List<Venue>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
