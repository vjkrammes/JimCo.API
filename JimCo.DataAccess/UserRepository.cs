using System.Data;
using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JimCo.DataAccess;
public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
  public UserRepository(IDatabase database) : base(database) { }

  public async Task<IEnumerable<UserEntity>> GetForRoleAsync(string rolename)
  {
    var sql = "select * from Users where JobTitles like CONCAT('%', @rolename, '%');";
    return await GetAsync(sql, new QueryParameter("rolename", rolename, DbType.String));
  }

  public async Task<UserEntity?> ReadAsync(string email)
  {
    var sql = "select * from Users where Email=@email;";
    return await ReadAsync(sql, new QueryParameter("email", email, DbType.String));
  }

  public async Task<UserEntity?> ReadForIdentifierAsync(string identifier)
  {
    var sql = "select * from Users where Identifier=@identifier;";
    return await ReadAsync(sql, new QueryParameter("identifier", identifier, DbType.String));
  }

  private async Task<DalResult> AddRolesAsync(UserEntity entity, params string[] roles)
  {
    if (entity is null)
    {
      return DalResult.NotFound;
    }
    try
    {
      var rolelist = JsonConvert.DeserializeObject<string[]>(entity.JobTitles)?.ToList() ?? new List<string>();
      foreach (var role in roles)
      {
        if (!rolelist.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
          rolelist.Add(role);
        }
      }
      entity.JobTitles = JsonConvert.SerializeObject(rolelist.ToArray());
      return await UpdateAsync(entity);
    }
    catch (Exception ex)
    {
      return DalResult.FromException(ex);
    }
  }

  public async Task<DalResult> AddRolesAsync(int id, params string[] roles)
  {
    if (id <= 0)
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(id);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await AddRolesAsync(user, roles);
  }

  public async Task<DalResult> AddRolesAsync(string email, params string[] roles)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(email);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await AddRolesAsync(user, roles);
  }

  private async Task<DalResult> RemoveRolesAsync(UserEntity entity, params string[] roles)
  {
    if (entity is null)
    {
      return DalResult.NotFound;
    }
    try
    {
      var rolelist = JsonConvert.DeserializeObject<string[]>(entity.JobTitles)?.ToList() ?? new List<string>();
      foreach (var role in roles)
      {
        if (rolelist.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
          rolelist.Remove(role);
        }
      }
      entity.JobTitles = JsonConvert.SerializeObject(rolelist);
      return await UpdateAsync(entity);
    }
    catch (Exception ex)
    {
      return DalResult.FromException(ex);
    }
  }

  public async Task<DalResult> RemoveRolesAsync(int id, params string[] roles)
  {
    if (id <= 0)
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(id);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await RemoveRolesAsync(user, roles);
  }

  public async Task<DalResult> RemoveRolesAsync(string email, params string[] roles)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(email);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await RemoveRolesAsync(user, roles);
  }

  private async Task<DalResult> ToggleRolesAsync(UserEntity entity, params string[] roles)
  {
    if (entity is null)
    {
      return DalResult.NotFound;
    }
    try
    {
      var rolelist = JsonConvert.DeserializeObject<string[]>(entity.JobTitles)?.ToList() ?? new List<string>();
      foreach (var role in roles)
      {
        if (rolelist.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
          rolelist.Remove(role);
        }
        else
        {
          rolelist.Add(role);
        }
      }
      entity.JobTitles = JsonConvert.SerializeObject(rolelist.ToArray());
      return await UpdateAsync(entity);
    }
    catch (Exception ex)
    {
      return DalResult.FromException(ex);
    }
  }

  public async Task<DalResult> ToggleRolesAsync(int id, params string[] roles)
  {
    if (id <= 0)
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(id);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await ToggleRolesAsync(user, roles);
  }

  public async Task<DalResult> ToggleRolesAsync(string email, params string[] roles)
  {
    if (string.IsNullOrWhiteSpace(email))
    {
      return DalResult.NotFound;
    }
    var user = await ReadAsync(email);
    if (user is null)
    {
      return DalResult.NotFound;
    }
    return await ToggleRolesAsync(user, roles);
  }

  private async Task<string?> ReadNameAsync(string column, string value)
  {
    var sql = $"select DisplayName from Users where {column}=@value;";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var result = await conn.ExecuteScalarAsync<string>(sql, new { value });
      return result;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<string?> ReadNameForIdAsync(int id)
  {
    var sql = $"select DisplayName from Users where Id={id};";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var result = await conn.ExecuteScalarAsync<string>(sql);
      return result;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<string?> ReadNameForIdentifierAsync(string identifier) => await ReadNameAsync("Identifier", identifier);

  public async Task<string?> ReadNameForEmailAsync(string email) => await ReadNameAsync("Email", email);
}