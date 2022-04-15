
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface ILineItemRepository : IRepository<LineItemEntity>
{
  Task<IEnumerable<LineItemEntity>> GetForOrderAsync(int orderid);
  Task<IEnumerable<LineItemEntity>> GetForProductAsync(int productid);
  Task<IEnumerable<LineItemEntity>> GetUnderstockedAsync();
  Task<bool> OrderHasLineItemsAsync(int orderid);
  Task<bool> ProductHasLineItemsAsync(int productid);
}
