using FluentValidation;
using Todo.Domain.Commands.Requests.Todo;

namespace Todo.Application.Validations.Todo
{
  public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
  {
  }
}
