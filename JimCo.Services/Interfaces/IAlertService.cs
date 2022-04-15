
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IAlertService : IDataService<AlertModel>
{
  Task<IEnumerable<AlertModel>> GetCurrentAsync();
  Task<IEnumerable<AlertModel>> GetForRoleAsync(params string[] roles);
  Task<IEnumerable<AlertModel>> GetCurrentForRoleAsync(params string[] roles);
  Task<int> DeleteExpiredAsync();
  Task<IEnumerable<string>> GetExpiredAsync();
}
