using MediatR;
using Todo.Domain.Commands.Responses.Todo;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Todo
{
  public class DeleteTaskCommand : IRequest<Result<DeleteTaskResponse>>
  {
    public string Id { get; set; }
  }
}