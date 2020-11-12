using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Shared;

namespace Todo.Application.Pipeline
{
  public class ValidateCommandPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TResponse : Result
  {
    private readonly IValidator<TRequest> _validator;

    public ValidateCommandPipeline(IValidator<TRequest> validator)
    {
      _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
      var validation = _validator.Validate(request);

      if (!validation.IsValid)
      {
        var response = Activator.CreateInstance<TResponse>();

        foreach (var error in validation.Errors)
          response.AddError(error.ErrorCode, error.PropertyName, error.ErrorMessage);

        return response;
      }

      return await next();
    }
  }
}