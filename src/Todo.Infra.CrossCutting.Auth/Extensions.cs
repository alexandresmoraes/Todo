using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Todo.Infra.CrossCutting.Auth.Stores;

namespace Todo.Infra.CrossCutting.Auth
{
  public static class Extensions
  {


    private static IdentityBuilder AddHibernateStores(
        this IdentityBuilder builder
    )
    {
      AddStores(builder.Services, builder.UserType, builder.RoleType);
      return builder;
    }

    private static void AddStores(
        IServiceCollection services,
        Type userType,
        Type roleType
    )
    {
      if (roleType != null)
      {
        var userStoreServiceType = typeof(IUserStore<>).MakeGenericType(userType);
        var userStoreImplType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
        services.AddScoped(userStoreImplType);
        services.AddScoped(userStoreServiceType, userStoreImplType);

        var roleStoreSvcType = typeof(IRoleStore<>).MakeGenericType(roleType);
        var roleStoreImplType = typeof(RoleStore<>).MakeGenericType(roleType);
        services.AddScoped(roleStoreImplType);
        services.AddScoped(roleStoreSvcType, roleStoreImplType);
      }
      else
      {
        var userStoreServiceType = typeof(IUserStore<>).MakeGenericType(userType);
        var userStoreImplType = typeof(UserOnlyStore<>).MakeGenericType(userType);
        services.AddScoped(userStoreImplType);
        services.AddScoped(userStoreServiceType, userStoreImplType);
      }

      
    }
  }
}