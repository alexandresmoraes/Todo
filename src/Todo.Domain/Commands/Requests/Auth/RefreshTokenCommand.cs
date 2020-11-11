using MediatR;
using Todo.Domain.Commands.Responses.Auth;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Requests.Auth
{
  public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
  }
}