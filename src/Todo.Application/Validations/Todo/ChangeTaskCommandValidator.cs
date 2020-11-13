using FluentValidation;
using Todo.Domain.Commands.Requests.Todo;

namespace Todo.Application.Validations.Todo
{
  public class ChangeTaskCommandValidator : AbstractValidator<ChangeTaskCommand>
  {
    public ChangeTaskCommandValidator()
    {
      RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Id não pode ser vazio.");

      RuleFor(x => x.Description)
        .NotEmpty().WithMessage("Descrição não pode ser vazio.");
    }
  }
}