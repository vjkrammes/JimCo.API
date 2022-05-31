
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface ISystemSettingsService
{
  Task<SystemSettingsModel?> GetSettingsAsync();
  Task<ApiError> UpdateAsync(SystemSettingsModel model);
}
