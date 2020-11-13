using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Commands.Requests.Todo;
using Todo.Domain.Commands.Responses.Todo;
using Todo.Domain.Shared;

namespace Todo.Presentation.WebApi.Controllers
{
  [Route("api/todo")]
  [ApiController]
  public class TodoController : ControllerBase
  {
    private readonly IMediator _mediator;

    public TodoController(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criação de novas tasks
    /// </summary>
    // POST api/todo
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<CreateTaskResponse>> Post(
      [FromBody] CreateTaskCommand command,
      CancellationToken cancellationToken = default
    ) => await _mediator.Send(command, cancellationToken);

    /// <summary>
    /// Alteração de tasks
    /// </summary>
    // PUT api/todo/{taskId}
    [HttpPut]
    [Route("{taskId}")]
    [ProducesResponseType(typeof(ChangeTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<ChangeTaskResponse>> Put(
      [FromBody] ChangeTaskCommand command,
      [FromRoute] string taskId,
      CancellationToken cancellationToken = default
    )
    {
      command.Id = taskId;
      return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Delete de tasks
    /// </summary>
    // DELETE api/todo/{taskId}
    [HttpDelete]
    [Route("{taskId}")]
    [ProducesResponseType(typeof(DeleteTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<Result<DeleteTaskResponse>> Put(
      [FromBody] DeleteTaskCommand command,
      [FromRoute] string taskId,
      CancellationToken cancellationToken = default
    )
    {
      command.Id = taskId;
      return await _mediator.Send(command, cancellationToken);
    }
  }
}