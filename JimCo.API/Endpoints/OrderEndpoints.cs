using JimCo.API.Infrastructure;
using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class OrderEndpoints
{
  public static void ConfigureOrderEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Order", GetOrders);
    app.MapGet("/api/v1/Order/ById/{orderid}", ById);
    app.MapGet("/api/v1/Order/ByEmail/{email}/{pinstring}", ByEmail);
    app.MapPost("/api/v1/Order/Create", CreateOrder);
    app.MapPost("/api/v1/Order/Online", CreateOnlineOrder).RequireAuthorization("JimCoEmployee");
    app.MapPost("/api/v1/Order/Checkout/{overrideIdentifier?}", CheckOut).RequireAuthorization("JimCoEmployee");
    app.MapPut("/api/v1/Order", Update).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Order/{orderId}", Delete).RequireAuthorization("ManagerPlusRequired");
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
  
  private static async Task<IResult> Delete(string orderId, IOrderService orderService, ILineItemService lineItemService)
  {
    if (string.IsNullOrWhiteSpace(orderId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "order id")));
    }
    var order = await orderService.ReadAsync(orderId);
    if (order is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "order", "id", orderId)));
    }
    if (await lineItemService.OrderHasLineItemsAsync(order.Id))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.CantDelete, "order", "line items")));
    }
    var result = await orderService.DeleteAsync(order);
    if (!result.Successful)
    {
      return Results.BadRequest(result);
    }
    return Results.Ok();
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

  private static async Task<IResult> CreateOnlineOrder([FromBody] OnlineOrderModel model, IOrderService orderService, IProductService productService,
    IUriHelper helper, IOptions<AppSettings> settings)
  {
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    if (model is null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Name) || model.Pin <= 0 || 
      model.Items is null || !model.Items.Any())
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var now = DateTime.UtcNow;
    var order = new OrderModel
    {
      Id = IdEncoder.EncodeId(0),
      Email = model.Email,
      Pin = IdEncoder.EncodeId(model.Pin),
      Name = model.Name,
      Address1 = string.Empty,
      Address2 = string.Empty,
      City = string.Empty,
      State = string.Empty,
      PostalCode = string.Empty,
      CreateDate = now,
      StatusDate = now,
      Status = OrderStatus.Pending,
      AgeRequired = model.AgeRequired,
    };
    var lineitems = new List<LineItemModel>();
    foreach (var item in model.Items)
    {
      var product = await productService.ReadAsync(item.ProductId);
      if (product is null)
      {
        return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "product", "id", item.ProductId)));
      }
      if (item.Price != product.Price)
      {
        var promo = product.Promotions?.FirstOrDefault(x => x.Price == item.Price);
        if (promo is null)
        {
          return Results.BadRequest(new ApiError(string.Format(Strings.InvalidPrice, product.Name)));
        }
      }
      var lineitem = new LineItemModel
      {
        Id = IdEncoder.EncodeId(0),
        OrderId = IdEncoder.EncodeId(0),
        ProductId = item.ProductId,
        Quantity = item.Quantity,
        Price = item.Price,
        AgeRequired = product.AgeRequired,
        StatusDate = now,
        Status = OrderStatus.Pending,
        Product = null,
      };
      lineitems.Add(lineitem);
    }
    var response = await orderService.CreateOnlineOrderAsync(order, lineitems);
    if (response.Successful)
    {
      var uri = helper.Create("Order", "ById", order.Id);
      return Results.Created(uri.ToString(), await orderService.ReadAsync(order.Id));
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> CheckOut([FromBody] OnlineOrderLineItem[] items, string? overrideidentifier, IProductService productService,
    IUserService userService)
  {
    bool over = false;
    if (items is null || !items.Any())
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "list of items")));
    }
    if (!string.IsNullOrWhiteSpace(overrideidentifier))
    {
      var user = await userService.ReadForIdentifierAsync(overrideidentifier);
      if (user is not null)
      {
        over = user.IsManager() || user.IsAdmin();
      }
    }
    foreach (var item in items)
    {
      var product = await productService.ReadAsync(item.ProductId);
      if (product is null)
      {
        return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "product", "id", item.Id)));
      }
      if (item.Quantity > product.Quantity && !over)
      {
        return Results.BadRequest(new ApiError(string.Format(Strings.Insufficient, product.Name)));
      }
      if (item.Price != product.Price)
      {
        var promo = product.Promotions?.FirstOrDefault(x => x.Price == item.Price);
        if (promo is null)
        {
          return Results.BadRequest(new ApiError(string.Format(Strings.BadPrice, product.Name)));
        }
      }
    }
    var products = 
      items.Select(x => new ProductSaleModel { ProductId = x.ProductId, Quantity = x.Quantity }).ToArray();
    var result = await productService.SellProductsAsync(products);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> Update([FromBody] OrderModel model, IOrderService orderService)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    if (!Enum.IsDefined(typeof(OrderStatus), (int)model.Status))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Invalid, "status")));
    }
    var result = await orderService.UpdateAsync(model, true);
    if (result.Successful)
    {
      return Results.Ok(model);
    }
    return Results.BadRequest(result);
  }
}
