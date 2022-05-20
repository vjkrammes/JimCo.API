namespace JimCo.API.Models;

public class AlertIdentityModel
{
  public string Identifier { get; set; }
  public string[] Roles { get; set; }

  public AlertIdentityModel()
  {
    Identifier = string.Empty;
    Roles = Array.Empty<string>();
  }
}
