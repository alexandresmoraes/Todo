using MediatR;
using Todo.Domain.Commands.Responses.Todo;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Todo
{
  public class ChangeTaskCommand : IRequest<Result<ChangeTaskResponse>>
  {
    public string Id { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
  }
}