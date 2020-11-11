using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public sealed class UserClaimMap : ClassMap<UserClaim>
  {
    public UserClaimMap()
    {
      Schema("auth");
      Table("user_claims");
      Id(x => x.Id);
      Map(x => x.ClaimType).Length(1024).Not.Nullable();
      Map(x => x.ClaimValue).Length(1024).Not.Nullable();
      Map(x => x.UserId).Not.Nullable();
    }
  }
}