using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Commands.Requests.Auth;
using Todo.Domain.Commands.Responses.Auth;
using Todo.Domain.Shared;

namespace Todo.Presentation.WebApi.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    /// <summary>
    /// Endpoint para verificar se o usuário está autorizado
    /// </summary>
    // GET api/auth
    [HttpGet]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
    public bool Get() => true;

    /// <summary>
    /// Validação de usuário e senha na geração de token
    /// </summary>
    // POST api/auth/login    
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<LoginResponse>> Login(
      [FromBody] LoginCommand loginCommand,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default
    ) => await mediator.Send(loginCommand, cancellationToken);

    /// <summary>
    /// Geração de token baseado no refresh token
    /// </summary>
    // POST api/auth/refreshtoken
    [AllowAnonymous]
    [HttpPost("refreshtoken")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<RefreshTokenResponse>> RefreshToken(
      [FromBody] RefreshTokenCommand refreshTokenCommand,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default
    ) => await mediator.Send(refreshTokenCommand, cancellationToken);
  }
}