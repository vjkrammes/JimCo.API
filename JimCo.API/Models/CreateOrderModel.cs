namespace JimCo.API.Models;

public class CreateOrderModel
{
  public string Email { get; set; }
  public int Pin { get; set; }
  public string Name { get; set; }
  public string Address1 { get; set; }
  public string Address2 { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string PostalCode { get; set; }
  public DateTime CreateDate { get; set; }
  public DateTime UpdateDate { get; set; }
  public List<CreateOrderLineItem> LineItems { get; set; }

  public CreateOrderModel()
  {
    Email = string.Empty;
    Pin = 0;
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    CreateDate = default;
    UpdateDate = default;
    LineItems = new();
  }
}
