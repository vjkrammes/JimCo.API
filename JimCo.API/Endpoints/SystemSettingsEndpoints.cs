using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace JimCo.API.Endpoints;

public static class SystemSettingsEndpoints
{
  public static void ConfigureSystemSettingsEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/System/Settings", GetSystemSettings);
    app.MapPut("/api/v1/System/Settings/Update", UpdateSystemSettings).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetSystemSettings(ISystemSettingsService systemSettingsService) =>
    Results.Ok(await systemSettingsService.GetSettingsAsync());

  private static async Task<IResult> UpdateSystemSettings([FromBody] SystemSettingsModel model, ISystemSettingsService systemSettingsService)
  {
    var response = await systemSettingsService.UpdateAsync(model);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }
}
