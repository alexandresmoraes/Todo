using MediatR;
using Todo.Domain.Commands.Responses.Todo;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Todo
{
  public class CreateTaskCommand : IRequest<Result<CreateTaskResponse>>
  {
    public string Description { get; set; }    
  }
}