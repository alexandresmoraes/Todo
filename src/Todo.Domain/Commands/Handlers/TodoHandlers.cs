using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Commands.Requests.Todo;
using Todo.Domain.Commands.Responses.Todo;
using Todo.Domain.IRepositories;
using Todo.Domain.Mapper;
using Todo.Domain.Shared;

namespace Todo.Domain.Commands.Handlers
{
  public class TodoHandlers :
    IRequestHandler<CreateTaskCommand, Result<CreateTaskResponse>>,
    IRequestHandler<ChangeTaskCommand, Result<ChangeTaskResponse>>,
    IRequestHandler<DeleteTaskCommand, Result<DeleteTaskResponse>>
  {
    private readonly IRepository _repository;
    private readonly IAutoMapper _autoMapper;

    public TodoHandlers(IRepository repository, IAutoMapper autoMapper)
    {
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _autoMapper = autoMapper ?? throw new ArgumentNullException(nameof(autoMapper));
    }

    public async Task<Result<CreateTaskResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var entity = _autoMapper.Map<Entities.Task.Task>(request);

      await _repository.SaveOrUpdateAsync(entity);

      return Result.Ok(new CreateTaskResponse
      {
        Id = entity.Id.ToString()
      });
    }

    public async Task<Result<ChangeTaskResponse>> Handle(ChangeTaskCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var entity = _autoMapper.Map<Entities.Task.Task>(request);

      await _repository.SaveOrUpdateAsync(entity);

      return Result.Ok(new ChangeTaskResponse
      {
        Id = entity.Id.ToString()
      });
    }

    public async Task<Result<DeleteTaskResponse>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var entity = await _repository.GetByIdAsync<Entities.Task.Task, Guid>(Guid.Parse(request.Id));

      await _repository.DeleteAsync(entity);

      return Result.Ok(new DeleteTaskResponse
      {
        Id = entity.Id.ToString()
      });
    }
  }
}