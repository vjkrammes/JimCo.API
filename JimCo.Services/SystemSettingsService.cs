
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class SystemSettingsService : ISystemSettingsService
{
  private readonly ISystemSettingsRepository _systemSettingsRepository;

  public SystemSettingsService(ISystemSettingsRepository systemSettingsRepository) => _systemSettingsRepository = systemSettingsRepository;

  public async Task<SystemSettingsModel?> GetSettingsAsync()
  {
    var entities = await _systemSettingsRepository.GetAsync();
    if (entities is null || !entities.Any())
    {
      return null;
    }
    SystemSettingsModel model = entities.First()!;
    model.CanDelete = false;
    return model;
  }

  public async Task<ApiError> UpdateAsync(SystemSettingsModel model)
  {
    SystemSettingsEntity entity = model!;
    return ApiError.FromDalResult(await _systemSettingsRepository.UpdateAsync(entity));
  }
}
