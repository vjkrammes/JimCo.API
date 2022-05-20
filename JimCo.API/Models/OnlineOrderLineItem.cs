namespace JimCo.API.Models;

public class OnlineOrderLineItem
{
  public Guid Id { get; set; }
  public string ProductId { get; set; }
  public int Quantity { get; set; }
  public decimal Price { get; set; }

  public OnlineOrderLineItem()
  {
    Id = Guid.Empty;
    ProductId = string.Empty;
    Quantity = 0;
    Price = 0M;
  }
}
