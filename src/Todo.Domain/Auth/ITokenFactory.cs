using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Dto.Auth;

namespace Todo.Domain.Auth
{
  public interface ITokenFactory
  {
    Task<AccessTokenDto> GenerateToken(string username, string token = null, string refreshToken = null, CancellationToken cancellationToken = default);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
  }
}
