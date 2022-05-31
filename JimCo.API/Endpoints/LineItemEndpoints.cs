using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class LineItemEndpoints
{
  public static void ConfigureLineItemEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/LineItem", GetLineItems);
    app.MapGet("/api/v1/LineItem/ById/{lineitemid}", ById);
    app.MapPut("/api/v1/LineItem/Status/{lineitemid}/{status}", SetStatus).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/LineItem/{lineitemid}", Delete).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetLineItems(ILineItemService lineItemService) => Results.Ok(await lineItemService.GetAsync());

  private static async Task<IResult> ById(string lineitemid, ILineItemService lineItemService)
  {
    var lineitem = await lineItemService.ReadAsync(lineitemid);
    return lineitem is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "line item", "id", lineitemid)))
      : Results.Ok(lineitem);
  }

  private static async Task<IResult> SetStatus(string lineItemId, int status, ILineItemService lineItemService)
  {
    var result = await lineItemService.UpdateStatusAsync(lineItemId, (OrderStatus)status);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> Delete(string lineitemid, ILineItemService lineItemService)
  {
    var item = await lineItemService.ReadAsync(lineitemid);
    if (item is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "line item", "id", lineitemid)));
    }
    var result = await lineItemService.DeleteAsync(item);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }
}