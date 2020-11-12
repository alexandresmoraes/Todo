using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Todo.Domain.Shared;

namespace Todo.Application.Mvc.Filters
{
  public class ResultFilterAttribute : Microsoft.AspNetCore.Mvc.Filters.ResultFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext context)
    {
      if (context.Result is ObjectResult objectResult)
      {
        if (objectResult.Value is Result result)
        {
          if (result.HasError)
          {
            context.Result = new BadRequestObjectResult(new Result(result.Errors));
          }
          else if (result.GetType().IsGenericType)
          {
            var data = result.GetType().GetProperty(nameof(Result<object>.Data)).GetValue(result, null);
            context.Result = new OkObjectResult(data);
          }
        }
      }

      base.OnResultExecuting(context);
    }
  }
}