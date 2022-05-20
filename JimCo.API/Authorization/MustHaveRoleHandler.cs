using System.Globalization;

using JimCo.API.Infrastructure;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;

using Newtonsoft.Json;

namespace JimCo.API.Authorization;

public class MustHaveRoleHandler : AuthorizationHandler<MustHaveRoleRequirement>
{
  private readonly IUserService _userService;
  private readonly IHttpContextAccessor _contextAccessor;

  public MustHaveRoleHandler(IUserService userService, IHttpContextAccessor contextAccessor)
  {
    _userService = userService;
    _contextAccessor = contextAccessor;
  }

  protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustHaveRoleRequirement requirement)
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
    var email = token.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
    if (string.IsNullOrWhiteSpace(email))
    {
      context.Fail(new(this, "Email claim not found"));
      return;
    }
    var user = await _userService.ReadForEmailAsync(email);
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
      context.Fail(new(this, "Deserialization of user roles failed"));
      return;
    }
    foreach (var job in jobs)
    {
      if (job.ToLower(CultureInfo.CurrentCulture).Equals(requirement.RoleName, StringComparison.OrdinalIgnoreCase))
      {
        context.Succeed(requirement);
        return;
      }
    }
    context.Fail(new(this, "User is not in the required role"));
  }
}
