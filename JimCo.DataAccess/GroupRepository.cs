using System.Data;
using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.Common.Enumerations;
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

  public async Task<GroupEntity?> ReadAsync(string name, int userid)
  {
    var sql = $"Select * from Groups where Name=@name and UserId={userid};";
    return await ReadAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  public async Task<GroupEntity?> ReadForNameAsync(string name)
  {
    var sql = "Select top(1) * from groups where Name=@name;";
    return await ReadAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  public async Task<bool> UserHasGroupsAsync(int userid)
  {
    var sql = $"Select count(*) from Groups where UserId={userid};";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var count = await conn.ExecuteScalarAsync<int>(sql);
      return count != 0;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<DalResult> RenameAsync(string name, string newname)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return new(DalErrorCode.Exception, new ArgumentNullException(nameof(name)));
    }
    if (string.IsNullOrWhiteSpace(newname))
    {
      return new(DalErrorCode.Exception, new ArgumentNullException(nameof(newname)));
    }
    if (await ReadForNameAsync(newname) is not null)
    {
      return new(DalErrorCode.Duplicate, new Exception($"There is already a group named '{newname}'"));
    }
    var sql = "Update Groups set Name=@newname where Name=@name;";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      await conn.ExecuteAsync(sql, new { newname, name });
      return DalResult.Success;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<DalResult> AddUserToGroupAsync(string name, int userid)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return DalResult.NotFound;
    }
    if (userid <= 0)
    {
      return new(DalErrorCode.Exception, new ArgumentException("User id must be greater than zero", nameof(userid)));
    }
    var existing = await ReadAsync(name, userid);
    if (existing is not null)
    {
      return DalResult.Duplicate;
    }
    var newgroup = new GroupEntity
    {
      Id = 0,
      Name = name,
      UserId = userid
    };
    try
    {
      return await InsertAsync(newgroup);
    }
    catch (Exception ex)
    {
      return new(DalErrorCode.Exception, ex);
    }
  }

  public async Task<DalResult> RemoveUserFromGroupAsync(string name, int userid)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return DalResult.NotFound;
    }
    if (userid <= 0)
    {
      return new(DalErrorCode.Exception, new ArgumentException("User id must be greater than zero", nameof(userid)));
    }
    var existing = await ReadAsync(name, userid);
    if (existing is null)
    {
      return DalResult.NotFound;
    }
    try
    {
      return await DeleteAsync(existing!);
    }
    catch (Exception ex)
    {
      return new(DalErrorCode.Exception, ex);
    }
  }
}
