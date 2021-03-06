namespace JimCo.API.Models;

public class CreateOrderLineItem
{
  public string ProductId { get; set; }
  public int Quantity { get; set; }
  public decimal Price { get; set; }

  public CreateOrderLineItem()
  {
    ProductId = string.Empty;
    Quantity = 0;
    Price = 0M;
  }
}
