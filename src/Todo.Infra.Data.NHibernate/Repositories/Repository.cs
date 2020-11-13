using NHibernate;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.IRepositories;

namespace Todo.Infra.Data.NHibernate.Repositories
{
  public class Repository : IRepository
  {
    private readonly ISession _session;

    private bool AutoFlushChanges { get; set; } = true;

    public Repository(ISession session)
    {
      _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<TEntity> GetByIdAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default) where TEntity : class
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (id == null)
      {
        throw new ArgumentNullException(nameof(id));
      }
      var entity = await _session.GetAsync<TEntity>(id);
      await FlushChanges();
      return entity;
    }

    public async Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (entity == null)
      {
        throw new ArgumentNullException(nameof(entity));
      }
      await _session.DeleteAsync(entity, cancellationToken);
      await FlushChanges();
    }

    public async Task<T> SaveOrUpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (entity == null)
      {
        throw new ArgumentNullException(nameof(entity));
      }
      await _session.SaveOrUpdateAsync(entity, cancellationToken);
      await FlushChanges();
      return entity;
    }

    protected async Task FlushChanges(CancellationToken cancellationToken = default)
    {
      if (AutoFlushChanges)
      {
        await _session.FlushAsync(cancellationToken);
        _session.Clear();
      }
    }

    
  }
}
