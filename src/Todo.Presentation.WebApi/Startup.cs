using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using Todo.Application;
using Todo.Application.Mvc.Filters;
using Todo.Domain.Commands.Handlers;
using Todo.Infra.CrossCutting.Auth;
using Todo.Infra.CrossCutting.Mapper;
using Todo.Infra.CrossCutting.Swagger;
using Todo.Infra.Data.NHibernate;

namespace Todo.Presentation.WebApi
{
  public class Startup
  {
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      _configuration = configuration;
      _environment = env;
    }    

    public void ConfigureServices(IServiceCollection services)
    {
      services
        .AddControllers()
        .AddNewtonsoftJson(opt => {
          opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
          opt.SerializerSettings.Formatting = _environment.IsDevelopment() ? Formatting.Indented : Formatting.None;
        })
        .AddXmlSerializerFormatters();

      services
        .ApplicationSetup()
        .AddAutoMapperr()
        .AddAuthentication(_configuration)
        .AddNHibernate(_configuration.GetConnectionString("Default"))
        .AddMediatR(Assembly.GetAssembly(typeof(AuthHandlers)))
        .AddCors()
        .AddSwagger()
        .AddMvc(opt =>
        {
          opt.RespectBrowserAcceptHeader = true;

          var xmlSerializer = opt.InputFormatters.OfType<XmlSerializerInputFormatter>().First();
          xmlSerializer.SupportedMediaTypes.Clear();
          xmlSerializer.SupportedMediaTypes.Add("application/xml");

          var patchSerializer = opt.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
          opt.InputFormatters.Remove(patchSerializer);

          var jsonSerializer = opt.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().First();
          jsonSerializer.SupportedMediaTypes.Clear();
          jsonSerializer.SupportedMediaTypes.Add("application/json");

          opt.Filters.Add(new ProducesAttribute("application/json", "application/xml"));
          opt.Filters.Add(new AuthorizeFilter());
          opt.Filters.Add<ResultFilterAttribute>();
          opt.Filters.Add<WebTransactionAttribute>();
        })
        .ConfigureApiBehaviorOptions(opt =>
        {
          opt.SuppressMapClientErrors = false;
          opt.SuppressModelStateInvalidFilter = false;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger()
         .UseHttpsRedirection()
         .UseRouting()
         .UseAuthentication()
         .UseAuthorization()
         .UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
        });
    }
  }
}
