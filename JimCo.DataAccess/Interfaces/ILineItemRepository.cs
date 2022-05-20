
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface ILineItemRepository : IRepository<LineItemEntity>
{
  Task<IEnumerable<LineItemEntity>> GetForOrderAsync(int orderid);
  Task<IEnumerable<LineItemEntity>> GetForProductAsync(int productid);
  Task<IEnumerable<LineItemEntity>> GetUnderstockedAsync();
  Task<DalResult> UpdateStatusAsync(int lineitemid, OrderStatus status);
  Task<bool> OrderHasLineItemsAsync(int orderid);
  Task<bool> ProductHasLineItemsAsync(int productid);
}
