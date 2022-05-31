namespace JimCo.Models;
public class CompleteGroup
{
  public string Id { get; set; }
  public string Name { get; set; }
  public UserModel[] Users { get; set; }

  public CompleteGroup()
  {
    Id = string.Empty;
    Name = string.Empty;
    Users = Array.Empty<UserModel>();
  }
}
