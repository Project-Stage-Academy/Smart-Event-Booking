using SmartEventBooking.Domain.Entities;

namespace SmartEventBooking.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}