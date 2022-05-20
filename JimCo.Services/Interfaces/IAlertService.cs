
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IAlertService : IDataService<AlertModel>
{
  Task<IEnumerable<AlertModel>> GetCurrentAsync();
  Task<IEnumerable<AlertModel>> GetForRoleAsync(params string[] roles);
  Task<IEnumerable<AlertModel>> GetCurrentForRoleAsync(params string[] roles);
  Task<IEnumerable<AlertModel>> GetForIdentifierAsync(string identifier, bool includeAcknowledged = false);
  Task<ApiError> Acknowledge(AlertModel model);
  Task<int> DeleteExpiredAsync();
  Task<ApiError> DeleteAllAsync(string identifier);
  Task<IEnumerable<string>> GetExpiredAsync();
}
