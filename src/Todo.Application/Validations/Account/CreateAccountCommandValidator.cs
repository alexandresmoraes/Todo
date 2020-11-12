using FluentValidation;
using Todo.Domain.Auth;
using Todo.Domain.Commands.Requests.Account;

namespace Todo.Application.Validations.Account
{
  public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
  {
    public CreateAccountCommandValidator(IAuthService authService)
    {
      RuleFor(x => x.Firstname)
        .NotEmpty().WithMessage("Primeiro nome do usuário não pode ser vazio.");

      RuleFor(x => x.Lastname)
        .NotEmpty().WithMessage("Sobrenome do usuário não pode ser vazio.");

      RuleFor(x => x.Email)
        .EmailAddress().WithMessage("Insira um email do usuário válido.")
        .NotEmpty().WithMessage("Email do usuário não pode ser vazio.")
        .MustAsync(async (email, ctoken) => !await authService.IsExistByEmailAsync(email, ctoken))
          .WithMessage("Email já cadastrado.");

      RuleFor(x => x.Username)
        .NotEmpty().WithMessage("Nome do usuário não pode ser vazio.")
        .MustAsync(async (username, ctoken) => !await authService.IsExistByUsernameAsync(username, ctoken))
          .WithMessage("Usuário já cadastrado.");

      RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Senha do usuário não pode ser vazio.");
    }
  }
}