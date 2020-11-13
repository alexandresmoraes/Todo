using FluentValidation;
using Todo.Domain.Commands.Requests.Todo;

namespace Todo.Application.Validations.Todo
{
  public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
  {
    public CreateTaskCommandValidator()
    {
      RuleFor(x => x.Description)
        .NotEmpty().WithMessage("Descrição da atividade não pode ser vazio.");
    }
  }
}