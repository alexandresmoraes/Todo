using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Todo.Domain.IUoW;

namespace Todo.Application.Mvc.Filters
{
  public class WebTransactionAttribute : System.Attribute, IAsyncActionFilter
  {
    private readonly IUnitOfWork _uow;

    public WebTransactionAttribute(IUnitOfWork uow)
    {
      _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var result = await next();

      if (result.Exception == null)
      {
        try
        {
          await _uow.CommitAsync();
        }
        catch (Exception e)
        {
          await _uow.RollbackAsync();
          throw new Exception("Erro ao commitar na base.", e);
        }
      }
      else
      {
        await _uow.RollbackAsync();
      }
    }
  }
}
