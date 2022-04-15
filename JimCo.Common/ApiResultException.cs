namespace JimCo.Common;
public abstract class ApiResultException : Exception
{
  public ApiResultException(string? message) : base(message) { }
}
