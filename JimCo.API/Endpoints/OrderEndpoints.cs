using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace JimCo.API.Endpoints;

public static class OrderEndpoints
{
  public static void ConfigureOrderEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Order", GetOrders);
    app.MapGet("/api/v1/Order/ById/{orderid}", ById);
    app.MapGet("/api/v1/Order/ByEmail/{email}/{pinstring}", ByEmail);
    app.MapPost("/api/v1/Order/Create", CreateOrder);
  }

  private static async Task<IResult> GetOrders(IOrderService orderService) => Results.Ok(await orderService.GetAsync());

  private static async Task<IResult> ById(string orderid, IOrderService orderService)
  {
    var order = await orderService.ReadAsync(orderid);
    return order is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "order", "id", orderid)))
      : Results.Ok(order);
  }

  private static async Task<IResult> ByEmail(string email, string pinstring, IOrderService orderService)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      return Results.BadRequest(string.Format(Strings.Invalid, "email"));
    }
    if (string.IsNullOrWhiteSpace(pinstring) || !int.TryParse(pinstring, out var pin) || pin <= 0)
    {
      return Results.BadRequest(string.Format(Strings.Invalid, "pin"));
    }
    var orders = await orderService.GetForEmailAndPinAsync(email, pin);
    if (orders is not null && orders.Any())
    {
      return Results.Ok(orders);
    }
    return Results.Ok(Array.Empty<OrderModel>());
  }

  private static async Task<IResult> CreateOrder([FromBody] CreateOrderModel model, IOrderService orderService, IProductService productService)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Email) || model.Pin <= 0 || string.IsNullOrWhiteSpace(model.Name) ||
      string.IsNullOrWhiteSpace(model.Address1) || string.IsNullOrWhiteSpace(model.City) || string.IsNullOrWhiteSpace(model.State) ||
      string.IsNullOrWhiteSpace(model.State) || string.IsNullOrWhiteSpace(model.PostalCode))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    if (model.LineItems is null || !model.LineItems.Any())
    {
      return Results.BadRequest(new ApiError(Strings.NoItems));
    }
    OrderModel order = new()
    {
      Id = IdEncoder.EncodeId(0),
      Email = model.Email,
      Pin = IdEncoder.EncodeId(model.Pin),
      Name = model.Name,
      Address1 = model.Address1,
      Address2 = model.Address2 ?? string.Empty,
      City = model.City,
      State = model.State,
      PostalCode = model.PostalCode,
      CreateDate = model.CreateDate,
      Status = OrderStatus.Pending,
      StatusDate = DateTime.UtcNow,
      AgeRequired = 0,
      CanDelete = false,
      LineItems = new()
    };
    foreach (var item in model.LineItems)
    {
      LineItemModel lineitem = new()
      {
        Id = IdEncoder.EncodeId(0),
        OrderId = IdEncoder.EncodeId(0),
        ProductId = item.ProductId,
        Price = item.Price,
        Quantity = item.Quantity,
        CanDelete = true,
        Status = OrderStatus.Pending,
        StatusDate = DateTime.UtcNow,
        AgeRequired = 0,
        Product = null
      };
      var product = await productService.ReadAsync(item.ProductId);
      if (product is null)
      {
        return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "product", "id", item.ProductId)));
      }
      if (item.Price != product.Price)
      {
        if (product.Promotions is not null && product.Promotions.Any())
        {
          if (!product.Promotions.Any(x => x.Price == item.Price))
          {
            return Results.BadRequest(Strings.BadPrice);
          }
        }
        else
        {
          return Results.BadRequest(Strings.BadPrice);
        }
      }
      lineitem.Product = product;
      lineitem.AgeRequired = product.AgeRequired;
      if (product.AgeRequired > order.AgeRequired)
      {
        order.AgeRequired = product.AgeRequired;
      }
      order.LineItems.Add(lineitem);
    }
    var result = await orderService.CreateOrderAsync(order, order.LineItems);
    if (result.Successful)
    {
      return Results.Ok(order);
    }
    return Results.BadRequest(result);
  }
}
