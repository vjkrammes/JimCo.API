using System.Data;
using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class GroupRepository : RepositoryBase<GroupEntity>, IGroupRepository
{
  public GroupRepository(IDatabase database) : base(database) { }

  public async Task<IEnumerable<GroupEntity>> GetAsync(string name)
  {
    var sql = "Select * from Groups where Name=@name;";
    return await GetAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  public async Task<IEnumerable<string>> GetGroupNamesAsync()
  {
    var sql = "select distinct Name from Groups order by Name asc;";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var result = await conn.QueryAsync<string>(sql);
      return result;
    }
    catch
    {
      return Array.Empty<string>();
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<GroupEntity?> ReadAsync(string name, string identity)
  {
    var sql = "Select * from Groups where Name=@name and Identifier=@identity";
    return await ReadAsync(sql,
      new QueryParameter("name", name, DbType.String), new QueryParameter("Identity", identity, DbType.String));
  }
}
