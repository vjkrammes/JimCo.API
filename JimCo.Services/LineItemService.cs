
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Zxcvbn;

namespace JimCo.Services;
public class LineItemService : ILineItemService
{
  private readonly ILineItemRepository _lineItemRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IProductRepository _productRepository;

  public LineItemService(ILineItemRepository lineItemRepository, IOrderRepository orderRepository, IProductRepository productRepository)
  {
    _lineItemRepository = lineItemRepository;
    _orderRepository = orderRepository;
    _productRepository = productRepository;
  }

  public async Task<int> CountAsync() => await _lineItemRepository.CountAsync();

  public async Task<ApiError> ValidateModelAsync(LineItemModel model, bool checkid = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.OrderId) || string.IsNullOrWhiteSpace(model.ProductId) || model.Quantity <= 0
      || model.Price <= 0M || model.AgeRequired < 0 || model.Status == OrderStatus.Unspecified)
    {
      return new(Strings.InvalidModel);
    }
    if (checkid)
    {
      var orderid = IdEncoder.DecodeId(model.OrderId);
      if (orderid <= 0 || (await _orderRepository.ReadAsync(orderid) is null))
      {
        return new(string.Format(Strings.NotFound, "order", "id", model.OrderId));
      }
    }
    var productid = IdEncoder.DecodeId(model.ProductId);
    if (productid <= 0 || (await _productRepository.ReadAsync(productid) is null))
    {
      return new(string.Format(Strings.NotFound, "product", "id", model.ProductId));
    }
    if (string.IsNullOrWhiteSpace(model.Id))
    {
      model.Id = IdEncoder.EncodeId(0);
    }
    if (checkid)
    {
      var decodedid = IdEncoder.DecodeId(model.Id);
      if (decodedid <= 0)
      {
        return new(string.Format(Strings.Invalid, "id"));
      }
    }
    if (model.StatusDate == default)
    {
      model.StatusDate = DateTime.UtcNow;
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(LineItemModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    LineItemEntity entity = model!;
    try
    {
      var result = await _lineItemRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(LineItemModel model)
  {
    var checkresult = await ValidateModelAsync(model, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    LineItemEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _lineItemRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(LineItemModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "line item", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _lineItemRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private static IEnumerable<LineItemModel> Finish(IEnumerable<LineItemEntity> entities)
  {
    var ret = entities.ToModels<LineItemModel, LineItemEntity>();
    ret.ForEach(x => x.CanDelete = true);
    return ret;
  }

  public async Task<IEnumerable<LineItemModel>> GetAsync()
  {
    var entities = await _lineItemRepository.GetAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<LineItemModel>> GetForOrderAsync(string orderid)
  {
    var id = IdEncoder.DecodeId(orderid);
    var entities = await _lineItemRepository.GetForOrderAsync(id);
    return Finish(entities);
  }

  public async Task<IEnumerable<LineItemModel>> GetForProductAsync(string productid)
  {
    var id = IdEncoder.DecodeId(productid);
    var entities = await _lineItemRepository.GetForProductAsync(id);
    return Finish(entities);
  }

  public async Task<IEnumerable<LineItemModel>> GetUnderstockedAsync()
  {
    var entities = await _lineItemRepository.GetUnderstockedAsync();
    return Finish(entities);
  }

  public async Task<LineItemModel?> ReadAsync(string lineItemId)
  {
    var id = IdEncoder.DecodeId(lineItemId);
    if (id <= 0)
    {
      return null;
    }
    var entity = await _lineItemRepository.ReadAsync(id);
    LineItemModel ret = entity!;
    ret.CanDelete = true;
    return ret;
  }

  public async Task<ApiError> UpdateStatusAsync(string id, OrderStatus status)
  {
    if (!Enum.IsDefined(typeof(OrderStatus), status))
{
      return new(string.Format(Strings.Invalid, "status"));
    }
    var pid = IdEncoder.DecodeId(id);
    return ApiError.FromDalResult(await _lineItemRepository.UpdateStatusAsync(pid, status));
  }

  public async Task<bool> OrderHasLineItemsAsync(string orderid)
  {
    var id = IdEncoder.DecodeId(orderid);
    return await _lineItemRepository.OrderHasLineItemsAsync(id);
  }

  public async Task<bool> ProductHasLineItemsAsync(string productid)
  {
    var id = IdEncoder.DecodeId(productid);
    return await _lineItemRepository.ProductHasLineItemsAsync(id);
  }
}
