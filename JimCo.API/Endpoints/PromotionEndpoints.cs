using JimCo.API.Infrastructure;
using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class PromotionEndpoints
{
  public static void ConfigurePromotionEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Promotion", GetPromotions);
    app.MapGet("/api/v1/Promotion/ForProduct/{productId}", GetForProduct);
    app.MapGet("/api/v1/Promotion/ById/{promotionid}", ById);
    app.MapPost("/api/v1/Promotion", Create).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Promotion", Update).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Promotion/Cancel/{promotionId}", Cancel).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Promotion/UnCancel/{promotionId}", UnCancel).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Promotion/{promotionId}", Delete).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Promotion/DeleteExpired/{productId}", DeleteExpired).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Promotion/DeleteAllExpired", DeleteAllExpired).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetPromotions(IPromotionService promotionService) => Results.Ok(await promotionService.GetAsync());

  private static async Task<IResult> GetForProduct(string productId, IPromotionService promotionService) =>
    Results.Ok(await promotionService.GetForProductAsync(productId));

  private static async Task<IResult> ById(string promotionid, IPromotionService promotionService)
  {
    var promotion = await promotionService.ReadAsync(promotionid);
    return promotion is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "promotion", "id", promotionid)))
      : Results.Ok(promotion);
  }

  private static async Task<IResult> Create([FromBody] PromotionModel model, IPromotionService promotionService, IUriHelper helper, IOptions<AppSettings> settings)
  {
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    if (model is null || string.IsNullOrWhiteSpace(model.Description))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var response = await promotionService.InsertAsync(model);
    if (response.Successful)
    {
      var uri = helper.Create("Promotion", "ById", model.Id);
      return Results.Created(uri.ToString(), model);
    }
    return Results.BadRequest(response);
  } 

  private static async Task<IResult> Update([FromBody] PromotionModel model, IPromotionService promotionService)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Description))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var response = await promotionService.UpdateAsync(model);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> Cancel(string promotionId, IPromotionService promotionService, IHttpContextAccessor accessor)
  {
    if (string.IsNullOrWhiteSpace(promotionId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "promotion", "id", promotionId ?? "")));
    }
    var token = accessor.HttpContext?.GetToken();
    if (token is null)
    {
      return Results.BadRequest(new ApiError(Strings.NotAuthenticated));
    }
    var identity = token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    if (string.IsNullOrWhiteSpace(identity))
    {
      return Results.BadRequest(new ApiError(Strings.NotAuthenticated));
    }
    var response = await promotionService.CancelAsync(promotionId, identity);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> UnCancel(string promotionId, IPromotionService promotionService)
  {
    if (string.IsNullOrWhiteSpace(promotionId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "promotion", "id", promotionId ?? "")));
    }
    var response = await promotionService.UnCancelAsync(promotionId);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> Delete(string promotionId, IPromotionService promotionService)
  {
    var model = await promotionService.ReadAsync(promotionId);
    if (model is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "promotion", "id", promotionId)));
    }
    var response = await promotionService.DeleteAsync(model);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> DeleteAllExpired(IPromotionService promotionService)
  {
    var result = await promotionService.DeleteAllExpiredAsync();
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> DeleteExpired(string productId, IPromotionService promotionService)
  {
    var result = await promotionService.DeleteExpiredAsync(productId);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }
}
