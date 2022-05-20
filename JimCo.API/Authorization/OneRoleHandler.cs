using JimCo.API.Infrastructure;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace JimCo.API.Authorization;

public class OneRoleHandler : AuthorizationHandler<OneRoleRequirement>
{
  private readonly IUserService _userService;
  private readonly IHttpContextAccessor _contextAccessor;

  public OneRoleHandler(IUserService userService, IHttpContextAccessor contextAccessor)
  {
    _userService = userService;
    _contextAccessor = contextAccessor;
  }

  protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, OneRoleRequirement requirement)
  {
    if (context is null || requirement is null)
    {
      throw new InvalidOperationException("Context and Requirement are required");
    }
    if (!(context.User?.Identity?.IsAuthenticated ?? false))
    {
      context.Fail(new(this, "User is not authenticated"));
      return;
    }
    var token = _contextAccessor.HttpContext?.Request.GetToken();
    if (token is null)
    {
      context.Fail(new(this, "No Token found"));
      return;
    }
    var identifier = token!.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    if (string.IsNullOrWhiteSpace(identifier))
    {
      context.Fail(new(this, "No Email claim found"));
      return;
    }
    var user = await _userService.ReadForIdentifierAsync(identifier);
    if (user is null)
    {
      context.Fail(new(this, "User not found"));
      return;
    }
    if (string.IsNullOrWhiteSpace(user.JobTitles))
    {
      context.Fail(new(this, "User has no roles assigned"));
      return;
    }
    var jobs = JsonConvert.DeserializeObject<string[]>(user.JobTitles);
    if (jobs is null || !jobs.Any())
    {
      context.Fail(new(this, "Deserialization of roles failed"));
      return;
    }
    foreach (var role in requirement.Roles)
    {
      if (jobs.Contains(role, StringComparer.OrdinalIgnoreCase))
      {
        context.Succeed(requirement);
        return;
      }
    }
    context.Fail(new(this, "User does not have any of the required roles"));
  }
}
