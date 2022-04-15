
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IPromotionRepository : IRepository<PromotionEntity>
{
  Task<IEnumerable<PromotionEntity>> GetForProductAsync(int productid);
  Task<IEnumerable<PromotionEntity>> GetCurrentForProductAsync(int productid);
  Task<IEnumerable<int>> GetCurrentIdsAsync();
  Task<int> GetProductIdAsync(int promotionid);
  Task<bool> ProductHasPromotionsAsync(int productid);
}
