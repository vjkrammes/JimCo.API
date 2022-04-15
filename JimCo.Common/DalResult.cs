
using JimCo.Common.Enumerations;

namespace JimCo.Common;
public sealed class DalResult
{
  public DalErrorCode ErrorCode { get; }
  public Exception? Exception { get; }

  public DalResult(DalErrorCode errorCode = DalErrorCode.NoError, Exception? exception = null)
  {
    ErrorCode = errorCode;
    Exception = exception;
  }

  public string? ErrorMessage => Exception is not null ? Exception.Innermost() : ErrorCode.GetDescriptionFromEnumValue();

  public bool Successful => ErrorCode == DalErrorCode.NoError && Exception is null;

  public static DalResult Duplicate => new(DalErrorCode.Duplicate);
  public static DalResult FromException(Exception? exception) => new(DalErrorCode.Exception, exception);
  public static DalResult NotAuthorized => new(DalErrorCode.NotAuthorized);
  public static DalResult NotFound => new(DalErrorCode.NotFound);
  public static DalResult Success => new();
}
