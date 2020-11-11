using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public sealed class UserLoginMap : ClassMap<UserLogin>
  {
    public UserLoginMap()
    {
      Schema("auth");
      Table("user_logins");
      CompositeId()
        .KeyProperty(x => x.LoginProvider,
                     kp => kp.Length(32))
        .KeyProperty(x => x.ProviderKey,
                     kp => kp.Length(32));
      Map(x => x.ProviderDisplayName).Length(32);
      Map(x => x.UserId).Not.Nullable();
    }
  }
}
