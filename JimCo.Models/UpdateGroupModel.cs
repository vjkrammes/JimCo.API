namespace JimCo.Models;
public class UpdateGroupModel
{
  public string Name { get; set; }
  public string[] Added { get; set; }
  public string[] Removed { get; set; }

  public UpdateGroupModel()
  {
    Name = string.Empty;
    Added = Array.Empty<string>();
    Removed = Array.Empty<string>();
  }
}
