using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Auth;
using Todo.Domain.Commands.Requests.Account;
using Todo.Domain.Commands.Responses.Account;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Handlers
{
  public class AccountHandlers :
    IRequestHandler<CreateAccountCommand, Result<CreateAccountResponse>>,
    IRequestHandler<ChangeAccountPasswordCommand, Result<ChangeAccountPasswordResponse>>
  {
    private readonly IAuthService _authService;

    public AccountHandlers(IAuthService authService)
    {
      _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    public async Task<Result<CreateAccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = await _authService.CreateUserAsync(request.Firstname, request.Lastname, request.Email, request.Username, request.Password);

      if (result.IsValid)
      {
        return Result.Ok(new CreateAccountResponse
        {
          Username = request.Username,
          Email = request.Email
        });
      }

      return Result.Fail<CreateAccountResponse>(result.Errors.ToArray());
    }

    public async Task<Result<ChangeAccountPasswordResponse>> Handle(ChangeAccountPasswordCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var username = _authService.GetUserName();

      if (!await _authService.CheckPasswordAsync(username, request.CurrentPassword))
      {
        return Result.Fail<ChangeAccountPasswordResponse>("Senha atual não confere.");
      }

      var result = await _authService.ChangePasswordAsync(username, request.CurrentPassword, request.Password, cancellationToken);

      if (result.IsValid)
      {
        return Result.Ok(new ChangeAccountPasswordResponse
        {
          Username = username
        });
      }

      return Result.Fail<ChangeAccountPasswordResponse>(result.Errors.ToArray());
    }
  }
}
