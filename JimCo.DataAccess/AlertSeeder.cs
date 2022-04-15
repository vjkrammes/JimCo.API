
using JimCo.DataAccess.Interfaces;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class AlertSeeder : IAlertSeeder
{
  public Task SeedAsync(IConfiguration configuration, string sectionName) => Task.CompletedTask;
}
