using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Todo.Infra.CrossCutting.Swagger
{
  public static class Extensions
  {
    private const string API_NAME = "Todo Api";
    private const string API_VERSION = "1.0.0.0";
    private const string API_VERSION_SHORT = "1";

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = API_NAME,
          Version = API_VERSION,
          Description = "Todo skills",
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = "Cabeçalho de autorização JWT usando o esquema Bearer.\r\n\r\n" +
          "Digite 'Bearer' [espaço] e, em seguida, seu Bearer token na entrada de texto abaixo.\r\n\r\n" +
          "Exemplo: \"Bearer {User_Token}\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header
            },
            new List<string>()
          }
        });

        var xmlPath = Path.Combine(AppContext.BaseDirectory, "Todo.Presentation.WebApi.xml");
        c.IncludeXmlComments(xmlPath);
      });

      return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
    {
      app.UseSwagger(c =>
      {
        c.RouteTemplate = "dev/{documentName}/docs.json";
      });

      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/dev/v1/docs.json", $"{API_NAME} V{API_VERSION_SHORT}");
        c.DocumentTitle = API_NAME;        
        c.RoutePrefix = "dev";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.DefaultModelsExpandDepth(-1);
      });

      return app;
    }
  }
}
