using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Pipeline;
using Todo.Application.Validations.Account;
using Todo.Application.Validations.Auth;
using Todo.Application.Validations.Todo;
using Todo.Domain.Commands.Requests.Account;
using Todo.Domain.Commands.Requests.Auth;
using Todo.Domain.Commands.Requests.Todo;

namespace Todo.Application
{
  public static class Extensions
  {
    public static IServiceCollection ApplicationSetup(this IServiceCollection services)
    {
      services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidateCommandPipeline<,>));

      services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
      services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
      services.AddScoped<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
      services.AddScoped<IValidator<ChangeAccountPasswordCommand>, ChangeAccountPasswordCommandValidator>();
      services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskCommandValidator>();
      services.AddScoped<IValidator<ChangeTaskCommand>, ChangeTaskCommandValidator>();
      services.AddScoped<IValidator<DeleteTaskCommand>, DeleteTaskCommandValidator>();

      return services;
    }

    public static IMvcBuilder AddValidations(this IMvcBuilder builder)
    {
      builder.AddFluentValidation(options =>
      {
        options.RegisterValidatorsFromAssemblyContaining<LoginCommandValidator>();
      });

      return builder;
    }
  }
}
