using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum CancelOrderResult
{
  [Description("Unspecified")]
  Unspecified = 0,
  [Description("Invalid order number")]
  InvalidOrderNumber = 1,
  [Description("Order not found")]
  OrderNotFound = 2,
  [Description("Order not cancellable")]
  OrderNotCancellable = 3,
  [Description("Database error - Please call customer support")]
  DatabaseError = 4,
  [Description("Order canceled successfully")]
  OrderCanceled = 999
}
