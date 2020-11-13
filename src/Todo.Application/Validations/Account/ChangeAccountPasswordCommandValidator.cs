using FluentValidation;
using Todo.Domain.Auth;
using Todo.Domain.Commands.Requests.Account;

namespace Todo.Application.Validations.Account
{
  public class ChangeAccountPasswordCommandValidator : AbstractValidator<ChangeAccountPasswordCommand>
  {
    public ChangeAccountPasswordCommandValidator(IAuthService authService)
    {
      RuleFor(x => x.CurrentPassword)
        .NotEmpty().WithMessage("Senha atual do usuário não pode ser vazio.");

      RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Senha nova do usuário não pode ser vazio.");

      RuleFor(x => x)
        .Must((x) => !x.Password.Equals(x.CurrentPassword)).WithMessage("A nova senha não pode ser igual a antiga.");
    }
  }
}