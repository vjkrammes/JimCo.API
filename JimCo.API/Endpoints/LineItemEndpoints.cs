using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class LineItemEndpoints
{
  public static void ConfigureLineItemEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/LineItem", GetLineItems);
    app.MapGet("/api/v1/LineItem/ById/{lineitemid}", ById);
  }

  private static async Task<IResult> GetLineItems(ILineItemService lineItemService) => Results.Ok(await lineItemService.GetAsync());

  private static async Task<IResult> ById(string lineitemid, ILineItemService lineItemService)
  {
    var lineitem = await lineItemService.ReadAsync(lineitemid);
    return lineitem is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "line item", "id", lineitemid)))
      : Results.Ok(lineitem);
  }
}
