using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class UserEndpoints
{
  public static void ConfigureUserEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/User", GetUsers);
    app.MapGet("/api/v1/User/ById/{userid}", ById);
    app.MapGet("/api/v1/User/ByEmail/{email}", ByEmail);
    app.MapGet("/api/v1/User/ByIdentifier/{identifier}", ByIdentifier);
  }

  private static async Task<IResult> GetUsers(IUserService userService) => Results.Ok(await userService.GetAsync());

  private static async Task<IResult> ById(string userid, IUserService userService)
  {
    var user = await userService.ReadAsync(userid);
    return user is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "id", userid)))
      : Results.Ok(user);
  }

  private static async Task<IResult> ByEmail(string email, IUserService userService)
  {
    var user = await userService.ReadForEmailAsync(email);
    return user is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "email", email)))
      : Results.Ok(user);
  }

  private static async Task<IResult> ByIdentifier(string identifier, IUserService userService)
  {
    var user = await userService.ReadForIdentifierAsync(identifier);
    return user is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "user", "identifier", identifier)))
      : Results.Ok(user);
  }
}
