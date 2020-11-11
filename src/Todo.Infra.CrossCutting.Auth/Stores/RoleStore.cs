using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.CrossCutting.Auth.Stores
{
  public class RoleStore<TRole> : IQueryableRoleStore<TRole>, IRoleClaimStore<TRole>
    where TRole : Role
  {
    private readonly ISession _session;

    private bool disposed;

    public bool AutoFlushChanges { get; set; } = true;

    public RoleStore(ISession session)
    {
      _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public void Dispose()
    {
      _session.Dispose();
      disposed = true;
    }

    public virtual async Task<IdentityResult> CreateAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      await _session.SaveAsync(role, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public virtual async Task<IdentityResult> UpdateAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      var exists = await Roles.AnyAsync(
        r => r.Id == role.Id,
        cancellationToken
      );
      if (!exists)
      {
        return IdentityResult.Failed(
          new IdentityError
          {
            Code = "RoleNotExist",
            Description = $"Role with {role.Id} does not exists."
          }
        );
      }
      role.ConcurrencyStamp = Guid.NewGuid().ToString("N");
      await _session.MergeAsync(role, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public virtual async Task<IdentityResult> DeleteAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      await _session.DeleteAsync(role, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public virtual Task<string> GetRoleIdAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      return Task.FromResult(role.Id.ToString());
    }

    public virtual Task<string> GetRoleNameAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(
      TRole role,
      string roleName,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      role.Name = roleName;
      return Task.CompletedTask;
    }

    public Task<string> GetNormalizedRoleNameAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      return Task.FromResult(role.NormalizedName);
    }

    public Task SetNormalizedRoleNameAsync(
      TRole role,
      string normalizedName,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      role.NormalizedName = normalizedName;
      return Task.CompletedTask;
    }

    public async Task<TRole> FindByIdAsync(
      string roleId,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var id = roleId;
      var role = await _session.GetAsync<TRole>(id, cancellationToken);
      return role;
    }

    public async Task<TRole> FindByNameAsync(
      string normalizedRoleName,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      return await Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
    }

    public virtual IQueryable<TRole> Roles => _session.Query<TRole>();

    private IQueryable<RoleClaim> RoleClaims => _session.Query<RoleClaim>();

    public virtual async Task<IList<Claim>> GetClaimsAsync(
      TRole role,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }

      var claims = await RoleClaims
        .Where(rc => rc.RoleId == role.Id)
        .Select(c => new Claim(c.ClaimType, c.ClaimValue))
        .ToListAsync(cancellationToken);
      return claims;
    }

    public virtual async Task AddClaimAsync(
      TRole role,
      Claim claim,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      if (claim == null)
      {
        throw new ArgumentNullException(nameof(claim));
      }
      var roleClaim = CreateRoleClaim(role, claim);
      await _session.SaveAsync(roleClaim, cancellationToken);
      await FlushChanges(cancellationToken);
    }

    public virtual async Task RemoveClaimAsync(
      TRole role,
      Claim claim,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (role == null)
      {
        throw new ArgumentNullException(nameof(role));
      }
      if (claim == null)
      {
        throw new ArgumentNullException(nameof(claim));
      }
      var claims = await RoleClaims
          .Where(rc => rc.RoleId == role.Id
              && rc.ClaimValue == claim.Value
              && rc.ClaimType == claim.Type
           )
           .ToListAsync(cancellationToken);
      foreach (var c in claims)
      {
        await _session.DeleteAsync(c, cancellationToken);
      }
      await FlushChanges(cancellationToken);
    }

    protected void ThrowIfDisposed()
    {
      if (disposed)
      {
        throw new ObjectDisposedException(GetType().Name);
      }
    }

    protected virtual RoleClaim CreateRoleClaim(
      TRole role,
      Claim claim
    ) => new RoleClaim
    {
      RoleId = role.Id,
      ClaimType = claim.Type,
      ClaimValue = claim.Value
    };

    private async Task FlushChanges(CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (AutoFlushChanges)
      {
        await _session.FlushAsync(cancellationToken);
        _session.Clear();
      }
    }
  }
}