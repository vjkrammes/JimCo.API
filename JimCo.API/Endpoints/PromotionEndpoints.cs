using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class PromotionEndpoints
{
  public static void ConfigurePromotionEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Promotion", GetPromotions);
    app.MapGet("/api/v1/Promotion/ById/{promotionid}", ById);
  }

  private static async Task<IResult> GetPromotions(IPromotionService promotionService) => Results.Ok(await promotionService.GetAsync());

  private static async Task<IResult> ById(string promotionid, IPromotionService promotionService)
  {
    var promotion = await promotionService.ReadAsync(promotionid);
    return promotion is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "promotion", "id", promotionid)))
      : Results.Ok(promotion);
  }
}
