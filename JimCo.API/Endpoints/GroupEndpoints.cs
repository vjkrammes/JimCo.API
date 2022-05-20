using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class GroupEndpoints
{
  public static void ConfigureGroupEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Group", GetGroups).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/ById/{groupId}", ById).RequireAuthorization("ManagerPlusRequired");
    app.MapGet("/api/v1/Group/ByName/{groupName}", ByName).RequireAuthorization("ManagerPlusRequired");
    app.MapPost("/api/v1/Group/Add/{groupName}/{identifier}", AddUser).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetGroups(IGroupService groupService) => Results.Ok(await groupService.GetAsync());

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

  private static async Task<IResult> AddUser(string groupName, string identifier, IGroupService groupService, IUserService userService, IUriHelper helper,
    IOptions<AppSettings> settings)
  {
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    if (string.IsNullOrWhiteSpace(groupName))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "group name")));
    }
    if (string.IsNullOrWhiteSpace(identifier))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "identifier")));
    }
    var user = await userService.ReadForIdentifierAsync(identifier);
    if (user is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "identifier", identifier)));
    }
    var group = new GroupModel
    {
      Id = IdEncoder.EncodeId(0),
      Name = groupName,
      Identifier = identifier
    };
    var response =await groupService.InsertAsync(group);
    if (response.Successful)
    {
      var uri = helper.Create("Group", "ById", group.Id);
      return Results.Created(uri.ToString(), group);
    }
    return Results.BadRequest(response);
  }
}
