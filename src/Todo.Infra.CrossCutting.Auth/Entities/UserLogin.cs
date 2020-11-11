using Microsoft.AspNetCore.Identity;
using System;

namespace Todo.Infra.CrossCutting.Auth.Entities
{
  public class UserLogin : IdentityUserLogin<Guid>
  {
    protected bool Equals(UserLogin other)
    {
      return LoginProvider == other.LoginProvider
          && ProviderKey == other.ProviderKey;
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }
      if (ReferenceEquals(this, obj))
      {
        return true;
      }
      if (obj.GetType() != GetType())
      {
        return false;
      }
      return Equals((UserLogin)obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = 0;
        hashCode = LoginProvider.GetHashCode();
        hashCode = (hashCode * 397) ^ ProviderKey.GetHashCode();
        return hashCode;
      }
    }
  }
}
