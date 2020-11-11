using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Commands.Requests.Account;
using Todo.Domain.Commands.Responses.Account;
using Todo.Domain.Shared;

namespace Todo.Presentation.WebApi.Controllers
{
  [Route("api/account")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    /// <summary>
    /// Criação de novos usuários
    /// </summary>
    // POST api/account
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<CreateAccountResponse>> Post(
      [FromBody] CreateAccountCommand command,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default
    ) => await mediator.Send(command, cancellationToken);

    /// <summary>
    /// Alterar a senha do usuário
    /// </summary>    
    // PUT api/account/password
    [HttpPut("password")]
    [ProducesResponseType(typeof(ChangeAccountPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<ChangeAccountPasswordResponse>> Put(
      [FromBody] ChangeAccountPasswordCommand command,
      [FromServices] IMediator mediator,
      CancellationToken cancellationToken = default
    ) => await mediator.Send(command, cancellationToken);
  }
}