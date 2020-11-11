using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Auth;
using Todo.Domain.Shared;
using Todo.Infra.CrossCutting.Auth.Entities;

namespace Todo.Infra.CrossCutting.Auth.Services
{
  public class AuthService : IAuthService
  {
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<Result> ChangePasswordAsync(string username, string currentPassword, string password, CancellationToken cancellationToken = default)
    {
      var result = new Result();

      var user = await GetUserByNameOrEmailAsync(username, cancellationToken);

      var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, password);

      foreach (var identityError in identityResult.Errors)
        result.AddError(identityError.Code, identityError.Description);

      return result;
    }    

    public async Task<Result> CreateUserAsync(string firstname, string lastname, string email, string username, string password, CancellationToken cancellationToken = default)
    {
      var result = new Result();

      var identityResult = await _userManager.CreateAsync(new User
      {
        Firstname = firstname,
        Lastname = lastname,
        UserName = username,
        Email = email
      }, password);

      foreach (var identityError in identityResult.Errors)
        result.AddError(identityError.Code, identityError.Description);

      return result;
    }

    private async Task<User> GetUserByNameOrEmailAsync(string username, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
    }
  }
}