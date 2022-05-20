
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IOrderService : IDataService<OrderModel>
{
  Task<ApiError> UpdateAsync(OrderModel model, bool online);
  Task<ApiError> CreateOrderAsync(OrderModel model, IEnumerable<LineItemModel>? items = null);
  Task<ApiError> CreateOnlineOrderAsync(OrderModel model, IEnumerable<LineItemModel>? items = null);
  Task<ApiError> UpdateOrderAsync(OrderModel model, IEnumerable<LineItemModel>? addedItems = null, IEnumerable<LineItemModel>? deletedItems = null);
  Task<IEnumerable<OrderModel>> GetForEmailAsync(string email);
  Task<IEnumerable<OrderModel>> GetForEmailAsync(string email, OrderStatus status);
  Task<IEnumerable<OrderModel>> GetForEmailAndPinAsync(string email, int pin);
  Task<IEnumerable<OrderModel>> GetOpenAsync();
  Task<IEnumerable<OrderModel>> GetPendingAsync();
  Task<IEnumerable<OrderModel>> GetCanceledAsync();
  Task<IEnumerable<OrderModel>> GetInProgressAsync();
  Task<IEnumerable<OrderModel>> GetShippedAsync();
  Task<IEnumerable<string>> GetOpenOrderIdsAsync();
  Task<ApiError> CancelOrderAsync(string orderid, bool byCustomer = true);
  Task<ApiError> FulfillAsync(OrderModel model);
}
