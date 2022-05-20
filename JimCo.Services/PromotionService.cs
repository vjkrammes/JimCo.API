
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class PromotionService : IPromotionService
{
  private readonly IPromotionRepository _promotionRepository;
  private readonly IProductRepository _productRepository;

  public PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
  {
    _promotionRepository = promotionRepository;
    _productRepository = productRepository;
  }

  public async Task<int> CountAsync() => await _promotionRepository.CountAsync();

  private async Task<ApiError> ValidateModelAsync(PromotionModel model, bool checkid = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.ProductId) || string.IsNullOrWhiteSpace(model.CreatedBy)
      || model.Price <= 0M || string.IsNullOrWhiteSpace(model.Description) || (model.LimitedQuantity && model.MaximumQuantity <= 0) 
      || (!model.LimitedQuantity && model.MaximumQuantity != 0) || model.MaximumQuantity < 0)
    {
      return new(Strings.InvalidModel);
    }
    if (model.CreatedOn == default)
    {
      model.CreatedOn = DateTime.UtcNow;
    }
    if (model.StopDate < model.StartDate)
    {
      return new(string.Format(Strings.Invalid, "stop date"));
    }
    var pid = IdEncoder.DecodeId(model.ProductId);
    if (pid <= 0 || await _productRepository.ReadAsync(pid) is null)
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
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(PromotionModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    PromotionEntity entity = model!;
    try
    {
      var result = await _promotionRepository.InsertAsync(entity);
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
  public async Task<ApiError> UpdateAsync(PromotionModel model)
  {
    var checkresult = await ValidateModelAsync(model, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    PromotionEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _promotionRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(PromotionModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "promotion", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _promotionRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private static IEnumerable<PromotionModel> Finish(IEnumerable<PromotionEntity> entities)
  {
    var models = entities.ToModels<PromotionModel, PromotionEntity>();
    models.ForEach(x => x.CanDelete = true);
    return models;
  }

  public async Task<IEnumerable<PromotionModel>> GetAsync()
  {
    var entities = await _promotionRepository.GetAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<PromotionModel>> GetForProductAsync(string productid)
  {
    var pid = IdEncoder.DecodeId(productid);
    var entities = await _promotionRepository.GetForProductAsync(pid);
    return Finish(entities);
  }

  public async Task<IEnumerable<PromotionModel>> GetCurrentForProductAsync(string productid)
  {
    var pid = IdEncoder.DecodeId(productid);
    var entities = await _promotionRepository.GetCurrentForProductAsync(pid);
    return Finish(entities);
  }

  public async Task<PromotionModel?> ReadAsync(string promotionid)
  {
    var pid = IdEncoder.DecodeId(promotionid);
    if (pid <= 0)
    {
      return null;
    }
    PromotionModel ret = (await _promotionRepository.ReadAsync(pid))!;
    if (ret is not null)
    {
      ret.CanDelete = true;
    }
    return ret;
  }

  public async Task<IEnumerable<string>> GetCurrentIdsAsync()
  {
    var ids = await _promotionRepository.GetCurrentIdsAsync();
    return ids.Select(x => IdEncoder.EncodeId(x));
  }

  public async Task<string> GetProductIdAsync(string promotionid)
  {
    var pid = IdEncoder.DecodeId(promotionid);
    var promo = await _promotionRepository.ReadAsync(pid);
    return promo is null ? IdEncoder.EncodeId(0) : IdEncoder.EncodeId(promo.ProductId);
  }

  public async Task<bool> ProductHasPromotionsAsync(string productid)
  {
    var pid = IdEncoder.DecodeId(productid);
    return await _promotionRepository.ProductHasPromotionsAsync(pid);
  }

  public async Task<ApiError> CancelAsync(string promotionId, string canceledBy)
  {
    var pid = IdEncoder.DecodeId(promotionId);
    return ApiError.FromDalResult(await _promotionRepository.CancelAsync(pid, canceledBy));
  }


  public async Task<ApiError> UnCancelAsync(string promotionId)
  {
    var pid = IdEncoder.DecodeId(promotionId);
    return ApiError.FromDalResult(await _promotionRepository.UnCancelAsync(pid));
  }

  public async Task<ApiError> DeleteAllExpiredAsync() => ApiError.FromDalResult(await _promotionRepository.DeleteAllExpiredAsync());

  public async Task<ApiError> DeleteExpiredAsync(string productId)
  {
    var pid = IdEncoder.DecodeId(productId);
    return ApiError.FromDalResult(await _promotionRepository.DeleteExpiredAsync(pid));
  }
}
