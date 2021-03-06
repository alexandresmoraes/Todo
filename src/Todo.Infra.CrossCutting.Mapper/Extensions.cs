﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Mapper;
using Todo.Infra.CrossCutting.Mapper.Profiles;

namespace Todo.Infra.CrossCutting.Mapper
{
  public static class Extensions
  {
    public static IServiceCollection AddAutoMapperr(this IServiceCollection services)
    {
      var mapConfig = new MapperConfiguration(cfg =>
      {
        cfg.AddMaps(typeof(DtoToResponse).Assembly);
      });

      mapConfig.AssertConfigurationIsValid();

      services.AddSingleton(mapConfig.CreateMapper());

      services.AddSingleton<IAutoMapper, AutoMapper>();

      return services;
    }
  }
}
