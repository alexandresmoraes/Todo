using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public class RoleMap : ClassMap<Role>
  {
    public RoleMap()
    {
      Schema("auth");
      Table("roles");
      Id(x => x.Id).GeneratedBy.GuidComb().Unique();
      Map(x => x.Name).Length(64).Not.Nullable().Unique();
      Map(x => x.NormalizedName).Length(64).Not.Nullable().Unique();
      Map(x => x.ConcurrencyStamp).Length(32);
    }
  }
}