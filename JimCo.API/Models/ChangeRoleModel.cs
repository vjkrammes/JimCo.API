namespace JimCo.API.Models;

public class ChangeRoleModel
{
  public string Identifier { get; set; }
  public bool IsVendor { get; set; }
  public bool IsEmployee { get; set; }
  public bool IsManager { get; set; }
  public bool IsAdmin { get; set; }

  public ChangeRoleModel()
  {
    Identifier = string.Empty;
    IsVendor = false;
    IsEmployee = false;
    IsManager = false;
    IsAdmin = false;
  }
}
