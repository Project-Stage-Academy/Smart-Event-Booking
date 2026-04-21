using SmartEventBooking.Application.DTOs;

namespace SmartEventBooking.Application.Abstractions.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}