using Microsoft.AspNetCore.Identity;
using System;

namespace Todo.Infra.CrossCutting.Auth.Entities
{
  public class UserToken : IdentityUserToken<Guid>
  {
    protected bool Equals(UserToken other)
    {
      return UserId == other.UserId
          && LoginProvider == other.LoginProvider
          && Name == other.Name;
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
      return Equals((UserToken)obj);
    }
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = UserId.GetHashCode();
        hashCode = (hashCode * 397) ^ LoginProvider.GetHashCode();
        hashCode = (hashCode * 397) ^ Name.GetHashCode();
        return hashCode;
      }
    }
  }
}
