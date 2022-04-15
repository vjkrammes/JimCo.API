using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class SystemSettingsEndpoints
{
  public static void ConfigureSystemSettingsEndpoints(this WebApplication app) => app.MapGet("/api/v1/System/Settings", GetSystemSettings);

  private static async Task<IResult> GetSystemSettings(ISystemSettingsService systemSettingsService) =>
    Results.Ok(await systemSettingsService.GetSettingsAsync());
}
