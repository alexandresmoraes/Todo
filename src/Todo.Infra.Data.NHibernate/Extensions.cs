using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Todo.Domain.IRepositories;
using Todo.Domain.IUoW;
using Todo.Infra.Data.NHibernate.Mapping.Auth;
using Todo.Infra.Data.NHibernate.Repositories;
using Todo.Infra.Data.NHibernate.UoW;

namespace Todo.Infra.Data.NHibernate
{
  public static class Extensions
  {
    public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
    {
      services.AddSingleton(Fluently.Configure()
        .Database(PostgreSQLConfiguration.PostgreSQL82
          .ConnectionString(connectionString)
          .AdoNetBatchSize(30)
#if DEBUG
          .ShowSql().FormatSql().AdoNetBatchSize(0)
#endif
        )
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMap>())
#if DEBUG
        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
#endif
        .BuildConfiguration()
        .BuildSessionFactory());

      services.AddScoped(provider => provider.GetService<ISessionFactory>().OpenSession());
      services.AddScoped(provider => provider.GetService<ISessionFactory>().OpenStatelessSession());

      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddScoped<IRepository, Repository>();

      return services;
    }
  }
}