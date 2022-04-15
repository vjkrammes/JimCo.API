using System.Data;

using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class CategoryRepository : RepositoryBase<CategoryEntity>, ICategoryRepository
{
  public CategoryRepository(IDatabase database) : base(database) { }

  public async Task<CategoryEntity?> ReadAsync(string name)
  {
    var sql = "select * from Categories where Name=@name;";
    return await ReadAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  public async Task<IEnumerable<CategoryEntity>> SearchAsync(string searchText)
  {
    var sql = "select * from Categories where Name like CONCAT('%',@name,'%');";
    return await GetAsync(sql, new QueryParameter("name", searchText, DbType.String));
  }
}
