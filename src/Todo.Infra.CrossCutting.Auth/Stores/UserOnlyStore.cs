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
  public class UserOnlyStore<TUser> : UserStoreBase<TUser, Guid, UserClaim, UserLogin, UserToken>, IProtectedUserStore<TUser>
    where TUser : User
  {
    private ISession _session;

    public bool AutoFlushChanges { get; set; } = true;

    public override IQueryable<TUser> Users => _session.Query<TUser>();

    private IQueryable<UserClaim> UserClaims => _session.Query<UserClaim>();

    private IQueryable<UserLogin> UserLogins => _session.Query<UserLogin>();

    private IQueryable<UserToken> UserTokens => _session.Query<UserToken>();

    public UserOnlyStore(
      ISession session,
      IdentityErrorDescriber describer
    ) : base(describer ?? new IdentityErrorDescriber())
    {
      this._session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public override async Task<IdentityResult> CreateAsync(
      TUser user,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      await _session.SaveAsync(user, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public override async Task<IdentityResult> UpdateAsync(
      TUser user,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      var exists = await Users.AnyAsync(
          u => u.Id.Equals(user.Id),
          cancellationToken
      );
      if (!exists)
      {
        return IdentityResult.Failed(
          new IdentityError
          {
            Code = "UserNotExist",
            Description = $"User with id {user.Id} does not exists!"
          }
        );
      }
      await _session.MergeAsync(user, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public override async Task<IdentityResult> DeleteAsync(
      TUser user,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      await _session.DeleteAsync(user, cancellationToken);
      await FlushChanges(cancellationToken);
      return IdentityResult.Success;
    }

    public override async Task<TUser> FindByIdAsync(
      string userId,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var id = ConvertIdFromString(userId);
      var user = await _session.GetAsync<TUser>(id, cancellationToken);
      return user;
    }

    public override async Task<TUser> FindByNameAsync(
      string normalizedUserName,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var user = await Users.FirstOrDefaultAsync(
          u => u.NormalizedUserName == normalizedUserName,
          cancellationToken
      );
      return user;
    }

    protected override async Task<TUser> FindUserAsync(
      Guid userId,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var user = await Users.FirstOrDefaultAsync(
          u => u.Id.Equals(userId),
          cancellationToken
      );
      return user;
    }

    protected override async Task<UserLogin> FindUserLoginAsync(
      Guid userId,
      string loginProvider,
      string providerKey,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var userLogin = await UserLogins.FirstOrDefaultAsync(
          ul => ul.UserId.Equals(userId) && ul.LoginProvider == loginProvider
              && ul.ProviderKey == providerKey,
          cancellationToken
      );
      return userLogin;
    }

    protected override async Task<UserLogin> FindUserLoginAsync(
      string loginProvider,
      string providerKey,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      var userLogin = await UserLogins.FirstOrDefaultAsync(
          ul => ul.LoginProvider == loginProvider
              && ul.ProviderKey == providerKey,
          cancellationToken
      );
      return userLogin;
    }

    public override async Task<IList<Claim>> GetClaimsAsync(
      TUser user,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      var claims = await UserClaims
          .Where(
              uc => uc.UserId.Equals(user.Id)
          )
          .Select(c => c.ToClaim())
          .ToListAsync(cancellationToken);
      return claims;
    }

    public override async Task AddClaimsAsync(
      TUser user,
      IEnumerable<Claim> claims,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      if (claims == null)
      {
        throw new ArgumentNullException(nameof(claims));
      }
      foreach (var claim in claims)
      {
        await _session.SaveAsync(CreateUserClaim(user, claim), cancellationToken);
      }
      await FlushChanges(cancellationToken);
    }

    public override async Task ReplaceClaimAsync(
      TUser user,
      Claim claim,
      Claim newClaim,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      if (claim == null)
      {
        throw new ArgumentNullException(nameof(claim));
      }
      if (newClaim == null)
      {
        throw new ArgumentNullException(nameof(newClaim));
      }
      var matchedClaims = await UserClaims.Where(
              uc => uc.UserId.Equals(user.Id) &&
                  uc.ClaimValue == claim.Value
                  && uc.ClaimType == claim.Type
          )
          .ToListAsync(cancellationToken);
      foreach (var matchedClaim in matchedClaims)
      {
        matchedClaim.ClaimType = newClaim.Type;
        matchedClaim.ClaimValue = newClaim.Value;
        await _session.UpdateAsync(matchedClaim, cancellationToken);
      }
      await FlushChanges(cancellationToken);
    }

    public override async Task RemoveClaimsAsync(
      TUser user,
      IEnumerable<Claim> claims,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      if (claims == null)
      {
        throw new ArgumentNullException(nameof(claims));
      }
      foreach (var claim in claims)
      {
        var matchedClaims = await UserClaims
            .Where(
                uc => uc.UserId.Equals(user.Id) &&
                    uc.ClaimValue == claim.Value
                    && uc.ClaimType == claim.Type
            )
            .ToListAsync(cancellationToken);
        foreach (var matchedClaim in matchedClaims)
        {
          await _session.DeleteAsync(matchedClaim, cancellationToken);
        }
      }
      await FlushChanges(cancellationToken);
    }

    public override async Task<IList<TUser>> GetUsersForClaimAsync(
      Claim claim,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (claim == null)
      {
        throw new ArgumentNullException(nameof(claim));
      }
      var query = from userclaims in UserClaims
                  join user in Users on userclaims.UserId equals user.Id
                  where userclaims.ClaimValue == claim.Value
                      && userclaims.ClaimType == claim.Type
                  select user;
      return await query.ToListAsync(cancellationToken);
    }

    protected override async Task<UserToken> FindTokenAsync(
      TUser user,
      string loginProvider,
      string name,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      var token = await UserTokens.FirstOrDefaultAsync(
          ut => ut.UserId.Equals(user.Id) &&
              ut.LoginProvider == loginProvider
              && ut.Name == name,
          cancellationToken);
      return token;
    }

    protected override async Task AddUserTokenAsync(UserToken token)
    {
      ThrowIfDisposed();
      if (token == null)
      {
        throw new ArgumentNullException(nameof(token));
      }
      await _session.SaveAsync(token);
      await FlushChanges(default);
    }

    protected override async Task RemoveUserTokenAsync(UserToken token)
    {
      ThrowIfDisposed();
      if (token == null)
      {
        throw new ArgumentNullException(nameof(token));
      }
      await _session.DeleteAsync(token);
      await FlushChanges(default);
    }

    public override async Task AddLoginAsync(
      TUser user,
      UserLoginInfo login,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      if (login == null)
      {
        throw new ArgumentNullException(nameof(login));
      }
      await _session.SaveAsync(CreateUserLogin(user, login), cancellationToken);
      await FlushChanges(cancellationToken);
    }

    public override async Task RemoveLoginAsync(
      TUser user,
      string loginProvider,
      string providerKey,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      var login = await FindUserLoginAsync(
          user.Id,
          loginProvider,
          providerKey,
          cancellationToken
      );
      if (login != null)
      {
        await _session.DeleteAsync(login, cancellationToken);
      }
    }

    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(
      TUser user,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      if (user == null)
      {
        throw new ArgumentNullException(nameof(user));
      }
      var userId = user.Id;
      var logins = await UserLogins
          .Where(l => l.UserId.Equals(userId))
          .Select(
              l => new UserLoginInfo(
                  l.LoginProvider,
                  l.ProviderKey,
                  l.ProviderDisplayName
              )
          )
          .ToListAsync(cancellationToken);
      return logins;
    }

    public override async Task<TUser> FindByEmailAsync(
      string normalizedEmail,
      CancellationToken cancellationToken = default
    )
    {
      cancellationToken.ThrowIfCancellationRequested();
      ThrowIfDisposed();
      return await Users.FirstOrDefaultAsync(
          u => u.NormalizedEmail == normalizedEmail,
          cancellationToken
      );
    }

    protected async Task FlushChanges(CancellationToken cancellationToken)
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
