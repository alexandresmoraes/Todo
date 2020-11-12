using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Shared;

namespace Todo.Domain.Auth
{
  public interface IAuthService
  {
    Task<Result> CreateUserAsync(string firstname, string lastname, string email, string username, string password, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(string username, string currentPassword, string password, CancellationToken cancellationToken = default);
    Task<bool> CheckPasswordAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<bool> IsExistByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsExistByEmailAsync(string email, CancellationToken cancellationToken = default);
    string GetUserName();
  }
}
