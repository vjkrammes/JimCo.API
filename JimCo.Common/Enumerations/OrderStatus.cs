using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum OrderStatus
{
  [Description("Unspecified")]
  Unspecified = 0,
  [Description("Pending")]
  Pending = 1,
  [Description("Open")]
  Open = 2,
  [Description("Canceled by customer")]
  CanceledByCustomer = 3,
  [Description("Canceled by store")]
  CanceledByStore = 4,
  [Description("In progress")]
  InProgress = 5,
  [Description("Shipped")]
  Shipped = 6,
  [Description("Back ordered")]
  BackOrdered = 7,
  [Description("Out of stock")]
  OutOfStock = 8
}
