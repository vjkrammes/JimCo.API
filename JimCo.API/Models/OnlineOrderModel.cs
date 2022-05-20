namespace JimCo.API.Models;

public class OnlineOrderModel
{
  public string Email { get; set; }
  public string Name { get; set; }
  public int Pin { get; set; }
  public int AgeRequired { get; set; }
  public OnlineOrderLineItem[] Items { get; set; }

  public OnlineOrderModel()
  {
    Email = string.Empty;
    Name = string.Empty;
    Pin = 0;
    AgeRequired = 0;
    Items = Array.Empty<OnlineOrderLineItem>();
  }
}
