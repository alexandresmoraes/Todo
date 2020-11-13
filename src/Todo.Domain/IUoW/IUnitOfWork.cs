using System.Threading;
using System.Threading.Tasks;

namespace Todo.Domain.IUoW
{
  public interface IUnitOfWork
  {
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
  }
}