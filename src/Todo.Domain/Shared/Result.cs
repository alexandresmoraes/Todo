using System.Collections.Generic;

namespace Todo.Domain.Shared
{
  public class Result
  {
    public static Result<TDataResponse> Ok<TDataResponse>(TDataResponse data)
      => new Result<TDataResponse>(data);

    public static Result<TDataResponse> Fail<TDataResponse>(string message)
      => new Result<TDataResponse>(message);

    public static Result<TDataResponse> Fail<TDataResponse>(ResultError[] errors)
    {
      var result = new Result<TDataResponse>();
      foreach (var error in errors)
        result.AddError(error);
      return result;
    }

    public bool IsValid => !HasError;
    public bool HasError => Errors.Count > 0;
    public List<ResultError> Errors { get; private set; } = new List<ResultError>();

    public Result() { }

    public Result(List<ResultError> errors)
    {
      foreach (var error in errors)
        AddError(error);
    }

    public Result(ResultError validation)
      : this()
    => AddError(validation);

    public Result(string messageValidation)
      : this(new ResultError(null, null, messageValidation))
    { }

    public Result AddError(ResultError validation)
    {
      Errors.Add(validation);
      return this;
    }
    public Result AddError(string code, string message)
      => AddError(new ResultError(code, null, message));
    public Result AddError(string code, string property, string message)
      => AddError(new ResultError(code, property, message));
  }

  public class Result<TDataResponse> : Result
  {
    public TDataResponse Data { get; private set; }


    public Result()
      : base()
    { }

    public Result(string message)
      : base(message)
    { }

    public Result(TDataResponse data)
      : base()
    => Data = data;

    public Result(ResultError[] errors)
      : base()
    {
      foreach (var error in errors)
        AddError(error);
    }
  }

  public class ResultError
  {
    public string Code { get; set; }
    public string Property { get; set; }
    public string Message { get; set; }

    public ResultError()
    { }

    public ResultError(string code, string property, string message)
    {
      Code = code ?? "Error";
      Property = property;
      Message = message;
    }
  }
}
