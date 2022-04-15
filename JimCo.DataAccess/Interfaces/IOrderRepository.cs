
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IOrderRepository : IRepository<OrderEntity>
{
  Task<DalResult> CreateAsync(OrderEntity order, IEnumerable<LineItemEntity>? items = null);
  Task<DalResult> UpdateAsync(OrderEntity order, IEnumerable<LineItemEntity>? added = null, IEnumerable<LineItemEntity>? removed = null);
  Task<IEnumerable<OrderEntity>> GetForEmailAsync(string email);
  Task<IEnumerable<OrderEntity>> GetForEmailAsync(string email, OrderStatus status);
  Task<IEnumerable<OrderEntity>> GetForEmailAndPinAsync(string email, int pin);
  Task<IEnumerable<OrderEntity>> GetOpenAsync();
  Task<IEnumerable<OrderEntity>> GetPendingAsync();
  Task<IEnumerable<OrderEntity>> GetCanceledAsync();
  Task<IEnumerable<OrderEntity>> GetInProgressAsync();
  Task<IEnumerable<OrderEntity>> GetShippedAsync();
  Task<IEnumerable<int>> GetOpenOrderIdsAsync();
  Task<DalResult> CancelOrderAsync(int orderid, bool byCustomer = true);
  Task<DalResult> FulfillAsync(int orderid);
}
