using MediatR;
using Todo.Domain.Commands.Responses.Account;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Account
{
  public class CreateAccountCommand : IRequest<Result<CreateAccountResponse>>
  {
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
  }
}