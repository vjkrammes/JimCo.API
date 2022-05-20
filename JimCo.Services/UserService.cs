
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class UserService : IUserService
{
  private readonly IUserRepository _userRepository;

  public UserService(IUserRepository userRepository) => _userRepository = userRepository;

  public async Task<int> CountAsync() => await _userRepository.CountAsync();

  private async Task<ApiError> ValidateModelAsync(UserModel model, bool checkid = false, bool update = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.DisplayName))
    {
      return new(Strings.InvalidModel);
    }
    if (model.DateJoined == default)
    {
      model.DateJoined = DateTime.UtcNow;
    }
    if (string.IsNullOrWhiteSpace(model.JobTitles))
    {
      model.JobTitles = "[]";
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
        return new(string.Format(Strings.InvalidModel, "id"));
      }
    }
    var existingfromemail = await _userRepository.ReadAsync(model.Email);
    if (update)
    {
      if (existingfromemail is not null && existingfromemail.Id != IdEncoder.DecodeId(model.Id))
      {
        return new(string.Format(Strings.Duplicate, "user", "email", model.Email));
      }
    }
    else if (existingfromemail is not null)
    {
      return new(string.Format(Strings.Duplicate, "user", "email", model.Email));
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(UserModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    UserEntity entity = model!;
    try
    {
      var result = await _userRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(UserModel model)
  {
    var checkresult = await ValidateModelAsync(model, true, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    UserEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _userRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(UserModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    if (!model.CanDelete)
    {
      return new(string.Format(Strings.CantDelete, "user", "orders"));
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "user", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _userRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<IEnumerable<UserModel>> GetAsync()
  {
    var entities = await _userRepository.GetAsync();
    var models = entities.ToModels<UserModel, UserEntity>();
    models.ForEach(x => x.CanDelete = !x.JobTitles.Contains("admin", StringComparison.OrdinalIgnoreCase));
    return models;
  }

  public async Task<IEnumerable<UserModel>> GetForRoleAsync(string rolename)
  {
    var entities = await _userRepository.GetForRoleAsync(rolename);
    var models = entities.ToModels<UserModel, UserEntity>();
    models.ForEach(x => x.CanDelete = !x.JobTitles.Contains("admin", StringComparison.OrdinalIgnoreCase));
    return models;
  }

  public async Task<IEnumerable<UserModel>> GetManagersAsync() => await GetForRoleAsync("manager");

  public async Task<IEnumerable<UserModel>> GetAdminsAsync() => await GetForRoleAsync("admin");

  private static UserModel? Finish(UserEntity? entity)
  {
    UserModel model = entity!;
    if (model is not null)
    {
      model.CanDelete = !model.JobTitles.Contains("admin", StringComparison.OrdinalIgnoreCase);
    }
    return model;
  }

  public async Task<UserModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    return Finish(await _userRepository.ReadAsync(decodedid));
  }

  public async Task<UserModel?> ReadForEmailAsync(string email) => Finish(await _userRepository.ReadAsync(email))!;

  public async Task<UserModel?> ReadForIdentifierAsync(string identifier) => Finish(await _userRepository.ReadForIdentifierAsync(identifier))!;

  public async Task<ApiError> AddRolesAsync(string email, params string[] roles) =>
    ApiError.FromDalResult(await _userRepository.AddRolesAsync(email, roles));

  public async Task<ApiError> RemoveRolesAsync(string email, params string[] roles) =>
    ApiError.FromDalResult(await _userRepository.RemoveRolesAsync(email, roles));

  public async Task<ApiError> ToggleRolesAsync(string email, params string[] roles) =>
    ApiError.FromDalResult(await _userRepository.ToggleRolesAsync(email, roles));
}
