
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class OrderService : IOrderService
{
  private readonly IOrderRepository _orderRepository;
  private readonly ILineItemRepository _lineItemRepository;

  public OrderService(IOrderRepository orderRepository, ILineItemRepository lineItemRepository)
  {
    _orderRepository = orderRepository;
    _lineItemRepository = lineItemRepository;
  }

  public async Task<int> CountAsync() => await _orderRepository.CountAsync();

  private static ApiError ValidateModalAsync(OrderModel model, bool checkid = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Pin) || model.Status == OrderStatus.Unspecified || 
      model.AgeRequired < 0 || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Address1) || string.IsNullOrWhiteSpace(model.City) ||
      string.IsNullOrWhiteSpace(model.State) || string.IsNullOrWhiteSpace(model.PostalCode))
    {
      return new(Strings.InvalidModel);
    }
    if (IdEncoder.DecodeId(model.Pin) <= 0)
    {
      return new(string.Format(Strings.Invalid, "pin"));
    }
    if (model.CreateDate == default)
    {
      model.CreateDate = DateTime.UtcNow;
    }
    if (model.StatusDate == default)
    {
      model.StatusDate = DateTime.UtcNow;
    }
    if (model.LineItems is null)
    {
      model.LineItems = new();
    }
    if (model.StatusDate < model.CreateDate)
    {
      return new(string.Format(Strings.Invalid, "status date"));
    }
    if (string.IsNullOrWhiteSpace(model.Id))
    {
      model.Id = IdEncoder.EncodeId(0);
    }
    if (model.Address2 is null)
    {
      model.Address2 = string.Empty;
    }
    if (checkid)
    {
      var decodedid = IdEncoder.DecodeId(model.Id);
      if (decodedid <= 0)
      {
        return new(string.Format(Strings.Invalid, "id"));
      }
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(OrderModel model)
  {
    var checkid = ValidateModalAsync(model);
    if (!checkid.Successful)
    {
      return checkid;
    }
    OrderEntity entity = model!;
    try
    {
      var result = await _orderRepository.InsertAsync(entity);
      if (result.Successful)
      {
        model.Id = IdEncoder.EncodeId(entity.Id);
      }
      return ApiError.FromDalResult(result);
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> UpdateAsync(OrderModel model)
  {
    var checkresult = ValidateModalAsync(model, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    OrderEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _orderRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(OrderModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    if (!model.CanDelete)
    {
      return new(string.Format(Strings.CantDelete, "order", "line items"));
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "order", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _orderRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private async Task<IEnumerable<OrderModel>> Finish(IEnumerable<OrderEntity> entities)
  {
    var models = entities.ToModels<OrderModel, OrderEntity>();
    foreach (var model in models)
    {
      model.CanDelete = !await _lineItemRepository.OrderHasLineItemsAsync(IdEncoder.DecodeId(model.Id));
    }
    return models;
  }

  public async Task<IEnumerable<OrderModel>> GetAsync()
  {
    var entities = await _orderRepository.GetAsync();
    return await Finish(entities);
  }

  public async Task<IEnumerable<OrderModel>> GetForEmailAsync(string email)
  {
    var entities = await _orderRepository.GetForEmailAsync(email);
    return await Finish(entities);
  }

  public async Task<IEnumerable<OrderModel>> GetForEmailAsync(string email, OrderStatus status)
  {
    var entities = await _orderRepository.GetForEmailAsync(email, status);
    return await Finish(entities);
  }

  public async Task<IEnumerable<OrderModel>> GetForEmailAndPinAsync(string email, int pin)
  {
    var entities = await _orderRepository.GetForEmailAndPinAsync(email, pin);
    return await Finish(entities);
  }

  private async Task<IEnumerable<OrderModel>> GetForStatusAsync(OrderStatus status)
  {
    var entities = status switch
    {
      OrderStatus.Open => await _orderRepository.GetOpenAsync(),
      OrderStatus.Pending => await _orderRepository.GetPendingAsync(),
      OrderStatus.CanceledByCustomer or OrderStatus.CanceledByStore => await _orderRepository.GetCanceledAsync(),
      OrderStatus.InProgress => await _orderRepository.GetInProgressAsync(),
      OrderStatus.Shipped => await _orderRepository.GetShippedAsync(),
      _ => await _orderRepository.GetAsync()
    };
    return await Finish(entities);
  }

  public async Task<IEnumerable<OrderModel>> GetOpenAsync() => await GetForStatusAsync(OrderStatus.Open);

  public async Task<IEnumerable<OrderModel>> GetPendingAsync() => await GetForStatusAsync(OrderStatus.Pending);

  public async Task<IEnumerable<OrderModel>> GetCanceledAsync() => await GetForStatusAsync(OrderStatus.CanceledByCustomer);

  public async Task<IEnumerable<OrderModel>> GetInProgressAsync() => await GetForStatusAsync(OrderStatus.InProgress);

  public async Task<IEnumerable<OrderModel>> GetShippedAsync() => await GetForStatusAsync(OrderStatus.Shipped);

  public async Task<IEnumerable<string>> GetOpenOrderIdsAsync()
  {
    var ids = await _orderRepository.GetOpenOrderIdsAsync();
    return ids.Select(x => IdEncoder.EncodeId(x));
  }

  public async Task<OrderModel?> ReadAsync(string orderid)
  {
    var id = IdEncoder.DecodeId(orderid);
    if (id <= 0)
    {
      return null;
    }
    var entity = await _orderRepository.ReadAsync(id);
    OrderModel ret = entity!;
    if (ret is not null)
    {
      ret.CanDelete = !await _lineItemRepository.OrderHasLineItemsAsync(id);
    }
    return ret;
  }

  public async Task<ApiError> CreateOrderAsync(OrderModel model, IEnumerable<LineItemModel>? items = null)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    OrderEntity entity = model!;
    List<LineItemEntity> entities = new();
    if (items is not null)
    {
      foreach (var item in items)
      {
        entities.Add(item!);
      }
    }
    var result = await _orderRepository.CreateAsync(entity, entities);
    if (result.Successful)
    {
      model.Id = IdEncoder.EncodeId(entity.Id);
    }
    return ApiError.FromDalResult(result);
  }

  public async Task<ApiError> UpdateOrderAsync(OrderModel model, IEnumerable<LineItemModel>? added = null, IEnumerable<LineItemModel>? deleted = null)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    OrderEntity entity = model!;
    List<LineItemEntity> addeditems = new();
    List<LineItemEntity> deleteditems = new();
    if (added is not null)
    {
      foreach (var a in added)
      {
        addeditems.Add(a!);
      }
    }
    if (deleted is not null)
    {
      foreach (var d in deleted)
      {
        deleteditems.Add(d!);
      }
    }
    return ApiError.FromDalResult(await _orderRepository.UpdateAsync(entity, addeditems, deleteditems));
  }

  public async Task<ApiError> CancelOrderAsync(string orderid, bool byCustomer = true)
  {
    var id = IdEncoder.DecodeId(orderid);
    if (id <= 0)
    {
      return new(string.Format(Strings.NotFound, "order", "id", orderid));
    }
    return ApiError.FromDalResult(await _orderRepository.CancelOrderAsync(id, byCustomer));
  }

  public async Task<ApiError> FulfillAsync(OrderModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    OrderEntity entity = model!;
    return ApiError.FromDalResult(await _orderRepository.FulfillAsync(entity.Id));
  }
}
