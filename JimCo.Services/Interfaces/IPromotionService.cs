
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IPromotionService : IDataService<PromotionModel>
{
  Task<IEnumerable<PromotionModel>> GetForProductAsync(string productid);
  Task<IEnumerable<PromotionModel>> GetCurrentForProductAsync(string productid);
  Task<IEnumerable<string>> GetCurrentIdsAsync();
  Task<string> GetProductIdAsync(string promotionid);
  Task<bool> ProductHasPromotionsAsync(string productid);
  Task<ApiError> CancelAsync(string promotionId, string canceledBy);
  Task<ApiError> UnCancelAsync(string promotionId);
  Task<ApiError> DeleteAllExpiredAsync();
  Task<ApiError> DeleteExpiredAsync(string productId);
}
