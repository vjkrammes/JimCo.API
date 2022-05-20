using System.Globalization;

using JimCo.API.Infrastructure;
using JimCo.API.Models;
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace JimCo.API.Endpoints;

public static class UserEndpoints
{
  public static void ConfigureUserEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/User", GetUsers).RequireAuthorization();
    app.MapGet("/api/v1/User/Name/{credential}/{value}", GetName).RequireAuthorization();
    app.MapGet("/api/v1/User/ById/{userid}", ById).RequireAuthorization();
    app.MapGet("/api/v1/User/ByEmail/{email}", ByEmail).RequireAuthorization();
    app.MapGet("/api/v1/User/ByIdentifier/{identifier}", ByIdentifier).RequireAuthorization();
    app.MapPost("/api/v1/User", Create).RequireAuthorization();
    app.MapPut("/api/v1/User/UpdateProfile", ChangeProfile).RequireAuthorization();
    app.MapPut("/api/v1/User/UpdateIdentifier", UpdateIdentifier);
  }

  private static async Task<IResult> GetUsers(IUserService userService) => Results.Ok(await userService.GetAsync());

  private static async Task<IResult> GetName(string credential, string value, IUserService userService)
  {
    if (string.IsNullOrWhiteSpace(credential) || string.IsNullOrWhiteSpace(value))
    {
      return Results.Ok(string.Empty);
    }
    var user = credential.ToLower(CultureInfo.CurrentCulture) switch
    {
      "email" => await userService.ReadForEmailAsync(value),
      "identifier" => await userService.ReadForIdentifierAsync(value),
      _ => null
    };
    if (user is null)
    {
      return Results.Ok(string.Empty);
    }
    return Results.Ok(string.IsNullOrWhiteSpace(user.DisplayName) ? user.ToString("fl") : user.DisplayName);
  }

  private static async Task<IResult> ById(string userid, IUserService userService)
  {
    var user = await userService.ReadAsync(userid);
    return user is null
      ? Results.BadRequest(new ApiError((int)DalErrorCode.NotFound, string.Format(Strings.NotFound, "user", "id", userid)))
      : Results.Ok(user);
  }

  private static async Task<IResult> ByEmail(string email, IUserService userService)
  {
    var user = await userService.ReadForEmailAsync(email);
    return user is null
      ? Results.BadRequest(new ApiError((int)DalErrorCode.NotFound, string.Format(Strings.NotFound, "user", "email", email)))
      : Results.Ok(user);
  }

  private static async Task<IResult> ByIdentifier(string identifier, IUserService userService)
  {
    var user = await userService.ReadForIdentifierAsync(identifier);
    return user is null
      ? Results.BadRequest(new ApiError((int)DalErrorCode.NotFound, string.Format(Strings.NotFound, "user", "identifier", identifier)))
      : Results.Ok(user);
  }

  private static async Task<IResult> Create(UserModel model, IUserService userService, IUriHelper uriHelper, IOptions<AppSettings> settings)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    uriHelper.SetBase(settings.Value.ApiBase);
    uriHelper.SetVersion(1);
    var result = await userService.InsertAsync(model);
    if (result.Successful)
    {
      var uri = uriHelper.Create("User", "ById", model.Id);
      return Results.Created(uri.ToString(), model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<ApiError> UpdateName(string identifier, string email, string? firstName, string? lastName, string? displayName, IUserService userService,
    IHttpContextAccessor contextAccessor)
  {
    if (string.IsNullOrWhiteSpace(identifier))
    {
      return new ApiError(string.Format(Strings.Required, "email address"));
    }
    var user = await userService.ReadForIdentifierAsync(identifier);
    if (user is null)
    {
      return new ApiError(string.Format(Strings.NotFound, "user", "identifier", identifier));
    }
    if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) &&
      string.IsNullOrWhiteSpace(displayName))
    {
      return ApiError.Success;
    }
    var context = contextAccessor?.HttpContext;
    if (context is null)
    {
      throw new InvalidOperationException("Cannot access HTTP Context");
    }
    var requesteridentifier = context.GetToken()?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    if (string.IsNullOrWhiteSpace(requesteridentifier))
    {
      return new ApiError(Strings.NotAuthenticated);
    }
    var requester = await userService.ReadForIdentifierAsync(requesteridentifier);
    if (requester is null)
    {
      return new ApiError(Strings.NotAuthenticated);
    }
    if (!string.Equals(requesteridentifier, user.Identifier, StringComparison.OrdinalIgnoreCase))
    {
      // user can change their own names, but a only a manager or admin can change someone else's
      if (!(requester.IsManager() || requester.IsAdmin()))
      {
        return new ApiError(Strings.NotAuthorized);
      }
    }
    if (!string.IsNullOrWhiteSpace(email))
    {
      user.Email = email;
    }
    if (!string.IsNullOrWhiteSpace(firstName))
    {
      user.FirstName = firstName;
    }
    if (!string.IsNullOrWhiteSpace(lastName))
    {
      user.LastName = lastName;
    }
    if (!string.IsNullOrWhiteSpace(displayName))
    {
      user.DisplayName = displayName;
    }
    var result = await userService.UpdateAsync(user);
    if (result.Successful)
    {
      return ApiError.Success;
    }
    return result;
  }

  private static async Task<ApiError> ChangeRole(ChangeRoleModel model, IUserService userService, IHttpContextAccessor contextAccessor)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Identifier))
    {
      return new ApiError(Strings.InvalidModel);
    }
    var context = contextAccessor.HttpContext;
    if (context is null)
    {
      return new ApiError(Strings.NotAuthenticated);
    }
    var requesteridentifier = context.GetToken()?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    if (string.IsNullOrWhiteSpace(requesteridentifier))
    {
      return new ApiError(Strings.NotAuthenticated);
    }
    var requester = await userService.ReadForIdentifierAsync(requesteridentifier);
    if (requester is null)
    {
      return new ApiError(Strings.NotAuthenticated);
    }
    if (!(requester.IsManager() || requester.IsAdmin()))
    {
      return new ApiError(Strings.NotAuthorized);
    }
    var user = await userService.ReadForIdentifierAsync(model.Identifier);
    if (user is null)
    {
      return new ApiError(string.Format(Strings.NotFound, "user", "identifier", model.Identifier));
    }
    if (user.IsAdmin() && !model.IsAdmin)
    {
      // attempt to demote admin, must make sure that there are at least 2 admins
      var admins = (await userService.GetAdminsAsync()).ToList();
      if (admins is null || admins.Count <= 1)
      {
        return new ApiError(Strings.CantDemoteAdmin);
      }
    }
    var roles = new List<string>();
    if (model.IsVendor)
    {
      roles.Add("Vendor");
    }
    if (model.IsEmployee)
    {
      roles.Add("Employee");
    }
    if (model.IsManager)
    {
      roles.Add("Manager");
    }
    if (model.IsAdmin)
    {
      roles.Add("Admin");
    }
    user.JobTitles = JsonConvert.SerializeObject(roles.ToArray());
    var result = await userService.UpdateAsync(user);
    if (result.Successful)
    {
      return ApiError.Success;
    }
    return result;
  }

  private static async Task<IResult> ChangeProfile(ChangeProfileModel model, IUserService userService, IHttpContextAccessor contextAccessor)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Identifier))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var result = await UpdateName(model.Identifier, model.Email, model.FirstName, model.LastName,
      model.DisplayName, userService, contextAccessor);
    if (result.Successful && model.ChangeRoles)
    {
      result = await ChangeRole(new ChangeRoleModel
      {
        Identifier = model.Identifier,
        IsVendor = model.IsVendor,
        IsEmployee = model.IsEmployee,
        IsManager = model.IsManager,
        IsAdmin = model.IsAdmin
      }, userService, contextAccessor);
    }
    return result.Successful ? Results.NoContent() : Results.BadRequest(result);
  }

  private static async Task<IResult> UpdateIdentifier([FromBody] UserModel model, IUserService userService, IHttpContextAccessor httpContextAccessor)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Identifier) || string.IsNullOrWhiteSpace(model.Email))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var user = await userService.ReadForEmailAsync(model.Email);
    if (user is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "email", model.Email)));
    }
    user.Identifier = model.Identifier;
    var result = await userService.UpdateAsync(user);
    if (result.Successful)
    {
      return Results.Ok(user);
    }
    return Results.BadRequest(result);
  }
}
