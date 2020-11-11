using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Auth;
using Todo.Domain.Dto.Settings;
using Todo.Infra.CrossCutting.Auth.Entities;
using Todo.Infra.CrossCutting.Auth.Services;
using Todo.Infra.CrossCutting.Auth.Stores;
using Todo.Infra.CrossCutting.Auth.Token;

namespace Todo.Infra.CrossCutting.Auth
{
  public static class Extensions
  {
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
      var authSettings = configuration.GetSection(nameof(AuthSettings));
      var jwtOptions = configuration.GetSection(nameof(JwtOptions));
      services.Configure<AuthSettings>(authSettings);
      services.Configure<JwtOptions>(jwtOptions);

      services
        .AddDefaultIdentity<User>()
        .AddRoles<Role>()
        .AddHibernateStores();

      services.AddScoped<IAuthService, AuthService>();
      services.AddScoped<ITokenFactory, TokenFactory>();

      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

      services
        .AddAuthentication(opt =>
        {
          opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
          opt.RequireHttpsMetadata = true;
          opt.SaveToken = true;
          opt.ClaimsIssuer = jwtOptions[nameof(JwtOptions.Issuer)];
          opt.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions[nameof(JwtOptions.Issuer)],

            ValidateAudience = true,
            ValidAudience = jwtOptions[nameof(JwtOptions.Audience)],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
          };

          opt.Events = new JwtBearerEvents
          {
            OnAuthenticationFailed = context =>
            {
              if (context.Exception is SecurityTokenExpiredException)
              {
                context.Response.Headers.Add("token-expired", "true");
              }
              return Task.CompletedTask;
            }
          };
        });

      services.Configure<IdentityOptions>(opt =>
      {
        opt.Password.RequiredLength = 6;
        opt.Password.RequireDigit = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireNonAlphanumeric = false;
      });

      return services;
    }

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