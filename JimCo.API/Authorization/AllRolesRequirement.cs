using JimCo.Common;

using Microsoft.AspNetCore.Authorization;

namespace JimCo.API.Authorization;

public class AllRolesRequirement : IAuthorizationRequirement
{
  public string[] Roles { get; }

  public AllRolesRequirement(params string[] roles) => Roles = roles.ArrayCopy();
}
