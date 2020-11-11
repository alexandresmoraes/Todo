using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public sealed class UserRoleMap : ClassMap<UserRole>
  {
    public UserRoleMap()
    {
      Schema("auth");
      Table("user_roles");
      CompositeId()
        .KeyProperty(x => x.UserId)
        .KeyProperty(x => x.RoleId);
    }
  }
}
