using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class SystemSettingsSeeder : ISystemSettingsSeeder
{
  private readonly ISystemSettingsRepository _systemSettingsRepository;

  public SystemSettingsSeeder(ISystemSettingsRepository systemSettingsRepository) => _systemSettingsRepository = systemSettingsRepository;

  public async Task SeedAsync(IConfiguration configuration, string sectionName)
  {
    var entities = await _systemSettingsRepository.GetAsync();
    if (entities is null || !entities.Any())
    {
      var result = await _systemSettingsRepository.InsertAsync(SystemSettingsEntity.Default);
      if (!result.Successful)
      {
        Console.WriteLine($"Failed to seed System Settings: {result.ErrorMessage}");
      }
    }
  }
}
