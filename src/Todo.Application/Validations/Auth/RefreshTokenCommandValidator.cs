using FluentValidation;
using Todo.Domain.Commands.Requests.Auth;

namespace Todo.Application.Validations.Auth
{
  public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
  {
    public RefreshTokenCommandValidator()
    {
      RuleFor(x => x.AccessToken)
        .NotEmpty().WithMessage("Access Token não pode ser vazio.");

      RuleFor(x => x.RefreshToken)
        .NotEmpty().WithMessage("Refresh Token não pode ser vazio.");
    }
  }
}