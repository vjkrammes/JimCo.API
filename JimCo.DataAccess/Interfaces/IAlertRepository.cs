
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IAlertRepository : IRepository<AlertEntity>
{
  Task<IEnumerable<AlertEntity>> GetForRoleAsync(params string[] rolenames);
  Task<IEnumerable<AlertEntity>> GetForIdentifierAsync(string identifier, bool includeAcknowledged = false);
  Task<IEnumerable<AlertEntity>> GetCurrentAsync();
  Task<IEnumerable<AlertEntity>> GetCurrentForRoleAsync(params string[] rolenames);
  Task<DalResult> Acknowledge(AlertEntity alertEntity);
  Task<DalResult> DeleteAllAsync(string identifier);
  Task<int> DeleteExpiredAsync();
  Task<IEnumerable<int>> GetExpiredAsync();
}
