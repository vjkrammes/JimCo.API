using System.Text;

using JimCo.Common.Enumerations;

namespace JimCo.Common;

[Serializable]
public class ApiError
{
  public int Code { get; }
  public string? Message { get; }
  public string[]? Messages { get; }

  public ApiError() : this(0, null, null) { }

  public ApiError(int code = 0, string? message = null, string[]? messages = null)
  {
    Code = code;
    Message = message ?? string.Empty;
    Messages = messages ?? Array.Empty<string>();
  }

  public ApiError(string? message) : this(0, message) { }

  public ApiError(string[]? messages) : this(0, messages: messages) { }

  public ApiError(string? message, string[]? messages) : this(0, message, messages) { }

  public bool Successful => Code == 0 && string.IsNullOrWhiteSpace(Message) && (Messages is null || !Messages.Any());

  public string[] Errors()
  {
    var ret = new List<string>();
    if (!string.IsNullOrWhiteSpace(Message))
    {
      ret.Add(Message);
    }
    if (Messages is not null && Messages.Any())
    {
      Array.ForEach(Messages, x => ret.Add(x));
    }
    return ret.ToArray();
  }

  public string ErrorMessage()
  {
    var messages = Errors();
    if (messages is null || !messages.Any())
    {
      return string.Empty;
    }
    var sb = new StringBuilder();
    Array.ForEach(messages, x => sb.AppendLine(x));
    return sb.ToString();
  }

  public static ApiError FromDalResult(DalResult result) => new((int)result.ErrorCode, result.Exception?.Innermost() ?? string.Empty);

  public static ApiError FromException(Exception exception) => new((int)DalErrorCode.Exception, exception?.Innermost() ?? string.Empty);

  public static ApiError Success => new();
}
