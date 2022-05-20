using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class GroupService : IGroupService
{
  private readonly IGroupRepository _groupRepository;
  private readonly IUserRepository _userRepository;

  public GroupService(IGroupRepository groupRepository, IUserRepository userRepository)
  {
    _groupRepository = groupRepository;
    _userRepository = userRepository;
  }

  public async Task<int> CountAsync() => await _groupRepository.CountAsync();

  private async Task<ApiError> ValidateModelAsync(GroupModel model, bool checkid = false, bool update = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Identifier))
    {
      return new(Strings.InvalidModel);
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
    var existing = await _groupRepository.ReadAsync(model.Name, model.Identifier);
    if (update)
    {
      if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
      {
        return new(string.Format(Strings.UserAlreadyInGroup, model.Name));
      }
    }
    else if (existing is not null)
    {
      return new(string.Format(Strings.UserAlreadyInGroup, model.Name));
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(GroupModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    GroupEntity entity = model!;
    try
    {
      var result = await _groupRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(GroupModel model)
  {
    var checkresult = await ValidateModelAsync(model, true, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    GroupEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _groupRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }
  
  public async Task<ApiError> DeleteAsync(GroupModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "group", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _groupRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private static IEnumerable<GroupModel> Finish(IEnumerable<GroupEntity> entities)
  {
    var models = entities.ToModels<GroupModel, GroupEntity>();
    models.ForEach(x => x.CanDelete = true);
    return models;
  }

  public async Task<IEnumerable<GroupModel>> GetAsync()
  {
    var entities = await _groupRepository.GetAsync();
    return Finish(entities);
  }

  public async Task<IEnumerable<GroupModel>> GetAsync(string name)
  {
    var entities = await _groupRepository.GetAsync(name);
    return Finish(entities);
  }

  public async Task<IEnumerable<string>> GetGroupNamesAsync() => await _groupRepository.GetGroupNamesAsync();

  private static GroupModel? Finish(GroupEntity? entity)
  {
    GroupModel model = entity!;
    if (model is not null)
    {
      model.CanDelete = true;
    }
    return model;
  }

  public async Task<GroupModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    var entity = await _groupRepository.ReadAsync(decodedid);
    return Finish(entity);
  }

  public async Task<GroupModel?> ReadAsync(string name, string identifier)
  {
    var entity = await _groupRepository.ReadAsync(name, identifier);
    return Finish(entity);
  }

  public async Task<PopulatedGroupModel?> ReadGroupAsync(string name)
  {
    var entities = await _groupRepository.GetAsync(name);
    if (entities is null || !entities.Any())
    {
      return null;
    }
    var ret = new PopulatedGroupModel(name);
    foreach (var entity in entities)
    {
      var user = await _userRepository.ReadForIdentifierAsync(entity.Identifier);
      if (user is not null)
      {
        UserModel model = user!;
        ret.Users.Add(model);
      }
    }
    return ret;
  }
}
