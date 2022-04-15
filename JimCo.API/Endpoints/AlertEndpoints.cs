using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class AlertEndpoints
{
  public static void ConfigureAlertEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Alert", GetAlerts);
    app.MapGet("/api/v1/Alert/ById/{alertid}", ById);
  }

  public static async Task<IResult> GetAlerts(IAlertService alertService) => Results.Ok(await alertService.GetAsync());

  public static async Task<IResult> ById(string alertid, IAlertService alertService)
  {
    var alert = await alertService.ReadAsync(alertid);
    return alert is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "alert", "id", alertid)))
      : Results.Ok(alert);
  }
}
