using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public sealed class UserTokenMap : ClassMap<UserToken>
  {
    public UserTokenMap()
    {
      Schema("auth");
      Table("user_tokens");
      CompositeId()
        .KeyProperty(x => x.UserId)
        .KeyProperty(x => x.LoginProvider,
                     kp => kp.Length(32))
        .KeyProperty(x => x.Name,
                     kp => kp.Length(32));
      Map(x => x.Value).Length(256);
    }
  }
}