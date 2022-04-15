
using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess.Interfaces;
public interface ISeeder<TEntity> where TEntity : class
{
  Task SeedAsync(IConfiguration configuration, string sectionName);
}
