namespace JimCo.DataAccess.Models;
public class ProductSeedModel
{
  public string Category { get; set; }
  public string Vendor { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string Sku { get; set; }
  public decimal Price { get; set; }
  public int AgeRequired { get; set; }
  public int Quantity { get; set; }
  public int ReorderLevel { get; set; }
  public int ReorderAmount { get; set; }
  public decimal Cost { get; set; }
  public bool Discontinued { get; set; }

  public ProductSeedModel()
  {
    Category = string.Empty;
    Vendor = string.Empty;
    Name = string.Empty;
    Description = string.Empty;
    Sku = string.Empty;
    Price = 0M;
    AgeRequired = 0;
    Quantity = 0;
    ReorderLevel = 0;
    ReorderAmount = 0;
    Cost = 0M;
    Discontinued = false;
  }
}
