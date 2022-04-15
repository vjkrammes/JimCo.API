
using JimCo.DataAccess.Interfaces;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class OrderSeeder : IOrderSeeder
{
  public Task SeedAsync(IConfiguration configuration, string sectionName) => Task.CompletedTask;
}
