
using Microsoft.AspNetCore.Authorization;

namespace JimCo.API.Authorization;

public class MustHaveRoleRequirement : IAuthorizationRequirement
{
  public string RoleName { get; }

  public MustHaveRoleRequirement(string rolename) => RoleName = rolename;
}
