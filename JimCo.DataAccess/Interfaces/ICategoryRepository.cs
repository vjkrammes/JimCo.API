
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface ICategoryRepository : IRepository<CategoryEntity>
{
  Task<CategoryEntity?> ReadAsync(string name);
  Task<IEnumerable<CategoryEntity>> SearchAsync(string searchText);
}
