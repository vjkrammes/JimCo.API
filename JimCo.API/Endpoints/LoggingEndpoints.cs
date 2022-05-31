using JimCo.API.Infrastructure;
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace JimCo.API.Endpoints;

public static class LoggingEndpoints
{
  public static void ConfigureLoggingEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Log", Get).RequireAuthorization("AdminRequired");
    app.MapGet("/api/v1/Log/Dates", GetDates).RequireAuthorization("AdminRequired");
    app.MapGet("/api/v1/Log/{date}/{level?}", GetDate).RequireAuthorization("AdminRequired");
    app.MapPost("/api/v1/Log", AddLog);
  }

  private static async Task<IResult> Get(ILogService logService)
  {
    var ret = await logService.GetAsync();
    return Results.Ok(ret);
  }

  private static async Task<IResult> GetDates(ILogService logService)
  {
    var ret = await logService.GetDatesAsync();
    return Results.Ok(ret ?? Array.Empty<DateTime>());
  }

  private static async Task<IResult> GetDate(DateTime date, Level? level, ILogService logService)
  {
    var ret = await logService.GetForDateAsync(date, level ?? Level.NoLevel);
    return Results.Ok(ret);
  }

  private static async Task<IResult> AddLog([FromBody] LogModel model, ILogService logService, IHttpContextAccessor accessor)
  {
    model.Ip = accessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    var token = accessor?.HttpContext?.GetToken();
    model.Identifier = token is not null ? token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? string.Empty : string.Empty;
    var response = await logService.InsertAsync(model);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }
}
