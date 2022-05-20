namespace JimCo.API.Models;

public class OnlineOrderResult
{
  public Guid Id { get; set; }
  public bool Successful { get; set; }
  public string Message { get; set; }
  public string[] Messages { get; set; }

  public OnlineOrderResult()
  {
    Id = Guid.Empty;
    Successful = false;
    Message = string.Empty;
    Messages = Array.Empty<string>();
  }
}
