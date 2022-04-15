namespace JimCo.DataAccess.Models;
public class VendorSeedModel
{
  public string Name { get; set; }
  public string Address1 { get; set; }
  public string Address2 { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string PostalCode { get; set; }
  public string Contact { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public string Fax { get; set; }

  public VendorSeedModel()
  {
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    Contact = string.Empty;
    Email = string.Empty;
    Phone = string.Empty;
    Fax = string.Empty;
  }
}
