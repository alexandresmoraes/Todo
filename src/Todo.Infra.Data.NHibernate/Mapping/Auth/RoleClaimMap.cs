using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public class RoleClaimMap : ClassMap<RoleClaim>
  {
    public RoleClaimMap()
    {
      Schema("auth");
      Table("claims");
      Id(x => x.Id);
      Map(x => x.ClaimType).Length(1024).Not.Nullable();
      Map(x => x.ClaimValue).Length(1024).Not.Nullable();
      Map(x => x.RoleId).Length(32).Not.Nullable();
    }
  }
}
