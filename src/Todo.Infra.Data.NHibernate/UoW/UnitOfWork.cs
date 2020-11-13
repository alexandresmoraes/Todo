using NHibernate;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.IUoW;

namespace Todo.Infra.Data.NHibernate.UoW
{
  public class UnitOfWork : IUnitOfWork, IDisposable
  {
    private readonly ISession _session;
    private readonly ITransaction _transaction;

    public UnitOfWork(ISession session)
    {
      _session = session ?? throw new ArgumentNullException(nameof(session));
      _transaction = _session.BeginTransaction();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return _transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
      if (_transaction != null) _transaction.Dispose();
      if (_session != null) _session.Dispose();
    }
  }
}