using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Auth;
using Todo.Domain.Commands.Requests.Auth;
using Todo.Domain.Commands.Responses.Auth;
using Todo.Domain.Mapper;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Handlers
{
  public class AuthHandlers :
    IRequestHandler<LoginCommand, Result<LoginResponse>>,
    IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
  {
    private readonly IAuthService _accountService;
    private readonly ITokenFactory _tokenFactory;
    private readonly IAutoMapper _autoMapper;

    public AuthHandlers(IAuthService accountService, ITokenFactory tokenFactory, IAutoMapper autoMapper)
    {
      _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
      _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
      _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (await _accountService.CheckPasswordAsync(request.Username, request.Password, cancellationToken))
      {
        var dto = await _tokenFactory.GenerateToken(request.Username, null, null, cancellationToken);

        return Result.Ok(_autoMapper.Map<LoginResponse>(dto));
      }

      return Result.Fail<LoginResponse>("Usuário ou senha inválidos");
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var claimsPrincipal = _tokenFactory.GetPrincipalFromExpiredToken(request.AccessToken);

      var dto = await _tokenFactory.GenerateToken(claimsPrincipal.Identity.Name, request.AccessToken, request.RefreshToken, cancellationToken);

      return Result.Ok(_autoMapper.Map<RefreshTokenResponse>(dto));
    }
  }
}
