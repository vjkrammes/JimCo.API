namespace JimCo.API.Models;

public class ChangeProfileModel
{
  public string Identifier { get; set; }
  public string Email { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string DisplayName { get; set; }
  public bool ChangeRoles { get; set; }
  public bool IsVendor { get; set; }
  public bool IsEmployee { get; set; }
  public bool IsManager { get; set; }
  public bool IsAdmin { get; set; }

  public ChangeProfileModel()
  {
    Identifier = string.Empty;
    Email = string.Empty;
    FirstName = string.Empty;
    LastName = string.Empty;
    DisplayName = string.Empty;
    ChangeRoles = false;
    IsVendor = false;
    IsEmployee = false;
    IsManager = false;
    IsAdmin = false;
  }
}
