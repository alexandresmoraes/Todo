using FluentValidation;
using Todo.Domain.Commands.Requests.Auth;

namespace Todo.Application.Validations.Auth
{
  public class LoginCommandValidator : AbstractValidator<LoginCommand>
  {
    public LoginCommandValidator()
    {
      RuleFor(x => x.Username)
        .NotEmpty().WithMessage("Usuário não pode ser vazio.");

      RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password não pode ser vazio.");
    }
  }
}