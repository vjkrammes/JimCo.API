
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IPromotionRepository : IRepository<PromotionEntity>
{
  Task<IEnumerable<PromotionEntity>> GetForProductAsync(int productid);
  Task<IEnumerable<PromotionEntity>> GetCurrentForProductAsync(int productid);
  Task<IEnumerable<int>> GetCurrentIdsAsync();
  Task<int> GetProductIdAsync(int promotionid);
  Task<bool> ProductHasPromotionsAsync(int productid);
  Task<DalResult> CancelAsync(int promotionId, string canceledBy);
  Task<DalResult> UnCancelAsync(int promotionId);
  Task<DalResult> DeleteAllExpiredAsync();
  Task<DalResult> DeleteExpiredAsync(int productId);
}
