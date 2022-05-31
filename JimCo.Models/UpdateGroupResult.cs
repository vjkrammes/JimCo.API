namespace JimCo.Models;
public class UpdateGroupResult
{
  public string Id { get; set; }
  public string Name { get; set; }
  public bool Success { get; set; }
  public string Result { get; set; }

  public UpdateGroupResult()
  {
    Id = string.Empty;
    Name = string.Empty;
    Success = false;
    Result = string.Empty;
  }
}
