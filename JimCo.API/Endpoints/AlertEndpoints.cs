using JimCo.API.Infrastructure;
using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class AlertEndpoints
{
  public static void ConfigureAlertEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Alert", GetAlerts).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Alert/ById/{alertid}", ById).RequireAuthorization();
    app.MapGet("/api/v1/Alert/ForIdentifier/{identifier}/{acknowledged?}", GetForIdentifier).RequireAuthorization();
    app.MapGet("/api/v1/Alert/ForRoles", GetForRoles).RequireAuthorization();
    app.MapPost("/api/v1/Alert/Current", GetCurrentAlerts).RequireAuthorization(); // post because JS fetch wont send body in get request
    app.MapPost("/api/v1/Alert", Create).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Alert", Update).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Alert/Acknowledge/{alertId}", Acknowledge).RequireAuthorization();
    app.MapDelete("/api/v1/Alert/Delete/{alertId}", Delete).RequireAuthorization();
    app.MapDelete("/api/v1/Alert/DeleteAll/{identifier}", DeleteAll).RequireAuthorization();
    app.MapDelete("/api/v1/Alert/DeleteExpired", DeleteExpired).RequireAuthorization("ManagerPlusRequired");
  }

  public static async Task<IResult> GetAlerts(IAlertService alertService) => Results.Ok(await alertService.GetAsync());

  public static async Task<IResult> GetCurrentAlerts([FromBody] AlertIdentityModel model, IAlertService alertService)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Identifier))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var alerts = (await alertService.GetForIdentifierAsync(model.Identifier, true))
      .ToList()
      .OrderByDescending(x => x.CreateDate);
    var notices = (await alertService.GetCurrentForRoleAsync(model.Roles))
      .ToList()
      .OrderByDescending(x => x.CreateDate);
    return Results.Ok(new { alerts, notices });
  }

  public static async Task<IResult> GetForIdentifier(string identifier, bool? acknowledged, IAlertService alertService) => 
    Results.Ok(await alertService.GetForIdentifierAsync(identifier, acknowledged ?? false));

  public static async Task<IResult> ById(string alertid, IAlertService alertService)
  {
    var alert = await alertService.ReadAsync(alertid);
    return alert is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "alert", "id", alertid)))
      : Results.Ok(alert);
  }

  public static async Task<IResult> GetForRoles([FromBody] string[] roles, IAlertService alertService)
  {
    if (roles is not null && roles.Any())
    {
      var ret = await alertService.GetForRoleAsync(roles);
      return Results.Ok(ret.ToArray());
    }
    return Results.Ok(Array.Empty<AlertModel>());
  }

  public static async Task<IResult> Create([FromBody] AlertModel model, IAlertService alertService, IUriHelper helper, IOptions<AppSettings> setting)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    helper.SetBase(setting.Value.ApiBase);
    helper.SetVersion(1);
    var result = await alertService.InsertAsync(model);
    if (result.Successful)
    {
      var uri = helper.Create("Alert", "ById", model.Id);
      return Results.Created(uri.ToString(), model);
    }
    return Results.BadRequest(result);
  }

  public static async Task<IResult> Update([FromBody] AlertModel model, IAlertService alertService)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Text))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var result = await alertService.UpdateAsync(model);
    if (!result.Successful)
    {
      return Results.BadRequest(result);
    }
    return Results.NoContent();
  }

  public static async Task<IResult> Delete(string alertId, IAlertService alertService)
  {
    if (string.IsNullOrWhiteSpace(alertId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "alert id")));
    }
    var alert = await alertService.ReadAsync(alertId);
    if (alert is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "alert", "id", alertId)));
    }
    if (alert.RequiresAcknowledgement && !alert.Acknowledged)
    {
      return Results.BadRequest(new ApiError(Strings.AckRequired));
    }
    var result = await alertService.DeleteAsync(alert);
    if (result.Successful)
    {
      return Results.NoContent();
    }
    return Results.BadRequest(result);
  }

  public static async Task<IResult> Acknowledge(string alertId, IAlertService alertService)
  {
    if (string.IsNullOrWhiteSpace(alertId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "alert id")));
    }
    var alert = await alertService.ReadAsync(alertId);
    if (alert is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "alert", "id", alertId)));
    }
    if (!alert.RequiresAcknowledgement)
    {
      return Results.BadRequest(new ApiError(Strings.NoAckNeeded));
    }
    if (alert.Acknowledged)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.AlertAck, alert.AcknowledgedOn.ToShortDateString())));
    }
    var result = await alertService.Acknowledge(alert);
    if (result.Successful)
    {
      return Results.NoContent();
    }
    return Results.BadRequest(result);
  }

  public static async Task<IResult> DeleteAll(string identifier, IAlertService alertService, IUserService userService, IHttpContextAccessor contextAccessor)
  {
    if (string.IsNullOrWhiteSpace(identifier))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "identifier")));
    }
    var token = contextAccessor.HttpContext?.GetToken();
    if (token is null)
    {
      return Results.BadRequest(new ApiError(Strings.NotAuthenticated));
    }
    var requester = token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    if (string.IsNullOrWhiteSpace(requester))
    {
      return Results.BadRequest(new ApiError(Strings.NotAuthenticated));
    }
    if (!string.Equals(requester, identifier, StringComparison.OrdinalIgnoreCase))
    {
      var req = await userService.ReadForIdentifierAsync(requester);
      if (req is null)
      {
        return Results.BadRequest(new ApiError(Strings.NotAuthenticated));
      }
      if (!(req.IsManager() || req.IsAdmin()))
      {
        return Results.BadRequest(new ApiError(Strings.NotAuthorized));
      }
    }
    var result = await alertService.DeleteAllAsync(identifier);
    if (result.Successful)
    {
      return Results.NoContent();
    }
    return Results.BadRequest(result);
  }

  public static async Task<IResult> DeleteExpired(IAlertService alertService)
  {
    await alertService.DeleteExpiredAsync();
    return Results.NoContent();
  }
}
