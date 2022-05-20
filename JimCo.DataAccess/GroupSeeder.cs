
using JimCo.DataAccess.Interfaces;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class GroupSeeder : IGroupSeeder
{
  public Task SeedAsync(IConfiguration configuration, string sectionName) => Task.CompletedTask;
}
