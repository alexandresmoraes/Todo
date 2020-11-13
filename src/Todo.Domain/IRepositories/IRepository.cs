using System.Threading;
using System.Threading.Tasks;

namespace Todo.Domain.IRepositories
{
  public interface IRepository
  {
    Task<TEntity> GetByIdAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default) where TEntity : class;
    Task<T> SaveOrUpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
    Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
  }
}
