
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface ILineItemService : IDataService<LineItemModel>
{
  Task<IEnumerable<LineItemModel>> GetForOrderAsync(string orderid);
  Task<IEnumerable<LineItemModel>> GetForProductAsync(string productid);
  Task<IEnumerable<LineItemModel>> GetUnderstockedAsync();
  Task<bool> OrderHasLineItemsAsync(string orderid);
  Task<bool> ProductHasLineItemsAsync(string productid);
}
