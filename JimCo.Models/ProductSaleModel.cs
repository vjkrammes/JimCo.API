namespace JimCo.Models;
public class ProductSaleModel
{
  public string ProductId { get; set; }
  public int Quantity { get; set; }

  public ProductSaleModel()
  {
    ProductId = string.Empty;
    Quantity = 0;
  }
}
