using System.Data;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;

public class VendorRepository : RepositoryBase<VendorEntity>, IVendorRepository
{
  public VendorRepository(IDatabase database) : base(database) { }

  public async Task<IEnumerable<VendorEntity>> PageVendorsAsync(int pageno, int pagesize, string columnName = "Id")
  {
    if (pageno <= 0)
    {
      pageno = 1;
    }
    if (pagesize <= 0)
    {
      pagesize = Constants.DefaultPageSize;
    }
    if (pagesize > Constants.MaxPageSize)
    {
      pagesize = Constants.MaxPageSize;
    }
    var offset = (pageno - 1) * pagesize;
    var sql = $"select * from Vendors order by {columnName} offset {offset} rows fetch next {pagesize} rows only;";
    return await GetAsync(sql, new QueryParameter("columnname", columnName, DbType.String));
  }

  public async Task<VendorEntity?> ReadAsync(string name)
  {
    var sql = "select * from Vendors where Name=@name;";
    return await ReadAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  public async Task<IEnumerable<VendorEntity>> SearchAsync(string searchText)
  {
    var sql = "select * from Vendors where Name like CONCAT('%', @searchText, '%');";
    return await GetAsync(sql, new QueryParameter("searchText", searchText, DbType.String));
  }

  public async Task<IEnumerable<VendorEntity>> SearchContactAsync(string searchText)
  {
    var sql = "select * from Vendors where Contact like CONCAT('%', @searchText, '%');";
    return await GetAsync(sql, new QueryParameter("searchText", searchText, DbType.String));
  }

  public async Task<IEnumerable<VendorEntity>> SearchEmailAsync(string searchText)
  {
    var sql = "select * from Vendors where Email like CONCAT('%', @searchText, '%');";
    return await GetAsync(sql, new QueryParameter("searchText", searchText, DbType.String));
  }

  public async Task<VendorEntity?> ReadForEmailAsync(string email)
  {
    var sql = "select * from Vendors where Email=@email;";
    return await ReadAsync(sql, new QueryParameter { Name = "email", Value = email, Type = DbType.String });
  }
}
