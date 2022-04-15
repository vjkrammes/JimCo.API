
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IAlertRepository : IRepository<AlertEntity>
{
  Task<IEnumerable<AlertEntity>> GetForRoleAsync(params string[] rolenames);
  Task<IEnumerable<AlertEntity>> GetCurrentAsync();
  Task<IEnumerable<AlertEntity>> GetCurrentForRoleAsync(params string[] rolenames);
  Task<int> DeleteExpiredAsync();
  Task<IEnumerable<int>> GetExpiredAsync();
}
