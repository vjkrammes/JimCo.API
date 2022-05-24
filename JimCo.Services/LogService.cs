
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class LogService : ILogService
{
  private readonly ILogRepository _logRepository;

  public LogService(ILogRepository logRepository) => _logRepository = logRepository;

  public async Task<int> CountAsync() => await _logRepository.CountAsync();

  public static ApiError ValidateModel(LogModel model)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Data))
    {
      return new(Strings.InvalidModel);
    }
    model.Id = IdEncoder.EncodeId(0);
    model.Timestamp = DateTime.UtcNow;
    if (!Enum.IsDefined(typeof(Level), model.Level))
    {
      return new(string.Format(Strings.Invalid, "log level"));
    }
    if (model.Ip is null)
    {
      model.Ip = string.Empty;
    }
    if (model.Identifier is null)
    {
      model.Identifier = string.Empty;
    }
    if (string.IsNullOrWhiteSpace(model.Source))
    {
      model.Source = "Unspecified";
    }
    if (string.IsNullOrWhiteSpace(model.Description))
    {
      model.Description = string.Empty;
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(LogModel model)
  {
    var checkresult = ValidateModel(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    LogEntity entity = model!;
    try
    {
      var result = await _logRepository.InsertAsync(entity);
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

  public Task<ApiError> UpdateAsync(LogModel model) => throw new NotSupportedException("Updates are not permitted to log entries");

  public Task<ApiError> DeleteAsync(LogModel model) => throw new NotSupportedException("Log entries cannot be deleted");

  private static IEnumerable<LogModel> Finish(IEnumerable<LogEntity> entities)
  {
    var models = entities.ToModels<LogModel, LogEntity>();
    models.ForEach(x => x.CanDelete = false);
    return models;
  }

  public async Task<IEnumerable<LogModel>> GetAsync()
  {
    var entities = await _logRepository.GetAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<LogModel>> GetForDateAsync(DateTime date, Level level = Level.NoLevel)
  {
    var entities = await _logRepository.GetForDateAsync(date, level);
    return Finish(entities);
  }

  public async Task<IEnumerable<DateTime>> GetDatesAsync() => await _logRepository.GetDatesAsync();

  public async Task<LogModel?> ReadAsync(string id)
  {
    var pid = IdEncoder.DecodeId(id);
    var entity = await _logRepository.ReadAsync(pid);
    if (entity is not null)
    {
      LogModel model = entity!;
      model.CanDelete = false;
      return model;
    }
    return null;
  }
}
