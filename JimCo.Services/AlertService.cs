
using System.Diagnostics.Metrics;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class AlertService : IAlertService
{
  private readonly IAlertRepository _alertRepository;

  public AlertService(IAlertRepository alertRepository) => _alertRepository = alertRepository;

  public async Task<int> CountAsync() => await _alertRepository.CountAsync();

  private static ApiError ValidateModel(AlertModel model, bool checkid = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Text))
    {
      return new(Strings.InvalidModel);
    }
    if (model.CreateDate == default)
    {
      model.CreateDate = DateTime.UtcNow;
    }
    if (!string.IsNullOrWhiteSpace(model.Identifier))
    {
      model.StartDate = default;
      model.EndDate = default;
    }
    else
    {
      if (model.EndDate < model.StartDate)
      {
        return new(Strings.ReversedDates);
      }
      if (model.EndDate < DateTime.UtcNow)
      {
        return new(Strings.AlertInPast);
      }
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

  public async Task<ApiError> InsertAsync(AlertModel model)
  {
    var checkresult = ValidateModel(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    AlertEntity entity = model!;
    try
    {
      var result = await _alertRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(AlertModel model)
  {
    var checkresult = ValidateModel(model, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    AlertEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _alertRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(AlertModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "alert", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _alertRepository.DeleteAsync(decodedid));
    } 
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private static IEnumerable<AlertModel> Finish(IEnumerable<AlertEntity> alerts)
  {
    var ret = alerts.ToModels<AlertModel, AlertEntity>();
    ret.ForEach(x => x.CanDelete = true);
    return ret.OrderByDescending(x => x.EndDate);
  }

  public async Task<IEnumerable<AlertModel>> GetAsync()
  {
    var entities = await _alertRepository.GetAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<AlertModel>> GetCurrentAsync()
  {
    var entities = await _alertRepository.GetCurrentAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<AlertModel>> GetForRoleAsync(params string[] roles)
  {
    var entities = await _alertRepository.GetForRoleAsync(roles);
    return Finish(entities);
  }

  public async Task<IEnumerable<AlertModel>> GetCurrentForRoleAsync(params string[] roles)
  {
    var entities = await _alertRepository.GetCurrentForRoleAsync(roles);
    return Finish(entities);
  }

  public async Task<IEnumerable<AlertModel>> GetForIdentifierAsync(string identifier, bool includeAcknowledged = false)
  {
    var entities = await _alertRepository.GetForIdentifierAsync(identifier, includeAcknowledged);
    return Finish(entities);
  }

  public async Task<AlertModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    var entity = await _alertRepository.ReadAsync(decodedid);
    AlertModel ret = entity!;
    ret.CanDelete = true;
    return ret;
  }

  public async Task<ApiError> Acknowledge(AlertModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    AlertEntity entity = model!;
    return ApiError.FromDalResult(await _alertRepository.Acknowledge(entity));
  }

  public async Task<int> DeleteExpiredAsync() => await _alertRepository.DeleteExpiredAsync();

  public async Task<IEnumerable<string>> GetExpiredAsync()
  {
    var ids = await _alertRepository.GetExpiredAsync();
    return ids.Select(x => IdEncoder.EncodeId(x));
  }

  public async Task<ApiError> DeleteAllAsync(string identifier) => ApiError.FromDalResult(await _alertRepository.DeleteAllAsync(identifier));
}
