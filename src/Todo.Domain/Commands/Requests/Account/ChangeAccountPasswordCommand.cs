using MediatR;
using Todo.Domain.Commands.Responses.Account;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Account
{
  public class ChangeAccountPasswordCommand : IRequest<Result<ChangeAccountPasswordResponse>>
  {
    public string CurrentPassword { get; set; }
    public string Password { get; set; }
  }
}