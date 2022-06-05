namespace JimCo.Models;
public class VendorUpdateModel
{
  public string Id { get; set; }
  public int ReorderAmount { get; set; }
  public decimal Cost { get; set; }

  public VendorUpdateModel()
  {
    Id = string.Empty;
    ReorderAmount = 0;
    Cost = 0;
  }
}
