using System.Threading;
using System.Threading.Tasks;

namespace SmartEventBooking.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
