using FluentNHibernate.Mapping;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.Data.NHibernate.Mapping.Auth
{
  public sealed class UserMap : ClassMap<User>
  {
    public UserMap()
    {
      Schema("auth");
      Table("users");
      Id(x => x.Id).GeneratedBy.GuidComb().Unique();
      Map(x => x.AccessFailedCount).Not.Nullable();
      Map(x => x.ConcurrencyStamp).Length(36);
      Map(x => x.Email).Length(256).Not.Nullable();
      Map(x => x.NormalizedEmail).Length(256)
        .Not.Nullable().Index("EmailIndex");
      Map(x => x.EmailConfirmed).Not.Nullable();
      Map(x => x.LockoutEnabled).Not.Nullable();
      Map(x => x.PasswordHash).Length(256);
      Map(x => x.PhoneNumber).Length(128);
      Map(x => x.PhoneNumberConfirmed).Not.Nullable();
      Map(x => x.TwoFactorEnabled).Not.Nullable();
      Map(x => x.UserName).Length(64).Not.Nullable().Unique();
      Map(x => x.NormalizedUserName).Length(64)
        .Not.Nullable().Index("UserNameIndex").Unique();
      Map(x => x.SecurityStamp).Length(64);
    }
  }
}
