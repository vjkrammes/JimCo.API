
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface ICategoryService : IDataService<CategoryModel>
{
  Task<IEnumerable<CategoryModel>> SearchAsync(string searchText);
  Task<CategoryModel?> ReadByNameAsync(string name);
}
