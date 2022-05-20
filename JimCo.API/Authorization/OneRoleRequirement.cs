using JimCo.Common;

using Microsoft.AspNetCore.Authorization;

namespace JimCo.API.Authorization;

public class OneRoleRequirement : IAuthorizationRequirement
{
  public string[] Roles { get; }

  public OneRoleRequirement(params string[] roles) => Roles = roles.ArrayCopy();
}
