using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class GroupEndpoints
{
  public static void ConfigureGroupEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Group", GetGroups).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/Names", GetGroupNames).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/Groups", GetPopulatedGroups).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/ById/{groupId}", ById).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/ByName/{groupName}", ByName).RequireAuthorization("ManagerPlusRequired");
    app.MapPost("/api/v1/Group/Add/{groupName}/{userId}", AddUser).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Group/Rename/{groupname}/{newname}", Rename).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Group/Update", UpdateGroup).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Group/Delete/{name}", Delete).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetGroups(IGroupService groupService) => Results.Ok(await groupService.GetAsync());

  private static async Task<IResult> GetPopulatedGroups(IGroupService groupService)
  {
    List<PopulatedGroupModel> ret = new();
    var names = await groupService.GetGroupNamesAsync();
    foreach (var name in names)
    {
      var group = await groupService.ReadGroupAsync(name);
      if (group is not null)
      {
        ret.Add(group);
      }
    }
    return Results.Ok(ret);
  }

  private static async Task<IResult> GetGroupNames(IGroupService groupService) => Results.Ok(await groupService.GetGroupNamesAsync());

  private static async Task<IResult> ById(string groupId, IGroupService groupService)
  {
    var model = await groupService.ReadAsync(groupId);
    if (model is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "group", "id", groupId)));
    }
    return Results.Ok(model);
  }

  private static async Task<IResult> ByName(string groupName, IGroupService groupService)
  {
    var model = await groupService.ReadGroupAsync(groupName);
    if (model is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "group", "name", groupName)));
    }
    return Results.Ok(model);
  }

  private static async Task<IResult> Rename(string groupname, string newname, IGroupService groupService)
  {
    if (string.IsNullOrWhiteSpace(groupname))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "group name")));
    }
    if (string.IsNullOrWhiteSpace(newname))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "new name")));
    }
    var result = await groupService.RenameAsync(groupname, newname);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> AddUser(string groupName, string userId, IGroupService groupService, IUserService userService, IUriHelper helper,
    IOptions<AppSettings> settings)
  {
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    if (string.IsNullOrWhiteSpace(groupName))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "group name")));
    }
    if (string.IsNullOrWhiteSpace(userId))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "identifier")));
    }
    var user = await userService.ReadAsync(userId);
    if (user is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "id", userId)));
    }
    var group = new GroupModel
    {
      Id = IdEncoder.EncodeId(0),
      Name = groupName,
      UserId = userId
    };
    var response =await groupService.InsertAsync(group);
    if (response.Successful)
    {
      var uri = helper.Create("Group", "ById", group.Id);
      return Results.Created(uri.ToString(), group);
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> Delete(string name, IGroupService groupService)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "name")));
    }
    var group = await groupService.ReadForNameAsync(name);
    if (group is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "group", "name", name)));
    }
    var response = await groupService.DeleteAsync(group);
    if (response.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(response);
  }

  private static async Task<IResult> UpdateGroup([FromBody] UpdateGroupModel model, IGroupService groupService, IUserService userService)
  {
    var ret = new List<UpdateGroupResult>();
    if (model is null || string.IsNullOrWhiteSpace(model.Name))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    if (model.Added is null)
    {
      model.Added = Array.Empty<string>();
    }
    if (model.Removed is null)
    {
      model.Removed = Array.Empty<string>();
    }
    if (model.Added.Length == 0 && model.Removed.Length == 0)
    {
      return Results.Ok(ret.ToArray());
    }
    foreach (var add in model.Added)
    {
      var user = await userService.ReadNameForIdAsync(add) ?? "Unknown";
      var result = await groupService.AddUserToGroupAsync(model.Name, add);
      if (result.Successful)
      {
        var r = new UpdateGroupResult
        {
          Id = add,
          Name = user,
          Success = true,
          Result = "Added successfully"
        };
        ret.Add(r);
      }
      else
      {
        var r = new UpdateGroupResult
        {
          Id = add,
          Name = user,
          Success = false,
          Result = result.ErrorMessage()
        };
        ret.Add(r);
      }
    }
    foreach (var remove in model.Removed)
    {
      var user = await userService.ReadNameForIdAsync(remove) ?? "Unknown";
      var result = await groupService.RemoveUserFromGroupAsync(model.Name, remove);
      if (result.Successful)
      {
        var r = new UpdateGroupResult
        {
          Id = remove,
          Name = user,
          Success = true,
          Result = "Removed successfully"
        };
        ret.Add(r);
      }
      else
      {
        var r = new UpdateGroupResult
        {
          Id = remove,
          Name = user,
          Success = false,
          Result = result.ErrorMessage()
        };
        ret.Add(r);
      }
    }
    return Results.Ok(ret);
  }
}
