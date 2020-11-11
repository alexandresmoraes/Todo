using MediatR;
using Todo.Domain.Commands.Responses.Auth;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Auth
{
  public class LoginCommand : IRequest<Result<LoginResponse>>
  {
    public string Username { get; set; }
    public string Password { get; set; }
  }
}