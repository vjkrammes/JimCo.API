
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface ILineItemService : IDataService<LineItemModel>
{
  Task<ApiError> ValidateModelAsync(LineItemModel model, bool checkid = false);
  Task<IEnumerable<LineItemModel>> GetForOrderAsync(string orderid);
  Task<IEnumerable<LineItemModel>> GetForProductAsync(string productid);
  Task<IEnumerable<LineItemModel>> GetUnderstockedAsync();
  Task<ApiError> UpdateStatusAsync(string id, OrderStatus status);
  Task<bool> OrderHasLineItemsAsync(string orderid);
  Task<bool> ProductHasLineItemsAsync(string productid);
}
