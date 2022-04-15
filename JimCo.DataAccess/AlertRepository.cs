using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess;
public class AlertRepository : RepositoryBase<AlertEntity>, IAlertRepository
{
  public AlertRepository(IDatabase database) : base(database)
  {

  }

  private static IEnumerable<AlertEntity> FilterForRoles(IEnumerable<AlertEntity> alerts, string[] rolenames)
  {
    var ret = new List<AlertEntity>();
    foreach (var alert in alerts)
    {
      var roles = JsonConvert.DeserializeObject<List<string>>(alert.Roles) ?? new();
      foreach (var rolename in rolenames)
      {
        if (roles.Contains(rolename))
        {
          ret.Add(alert);
        }
      }
    }
    return ret;
  }

  public async Task<IEnumerable<AlertEntity>> GetForRoleAsync(params string[] rolenames)
  {
    var alerts = await GetAsync();
    return FilterForRoles(alerts, rolenames);
  }

  public async Task<IEnumerable<AlertEntity>> GetCurrentAsync()
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"select * from Alerts where StartDate <= '{now}' and EndDate >= '{now}';";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<AlertEntity>> GetCurrentForRoleAsync(params string[] rolenames)
  {
    var alerts = await GetCurrentAsync();
    return FilterForRoles(alerts, rolenames);
  }

  public async Task<int> DeleteExpiredAsync()
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"delete from Alerts where EndDate < '{now}';";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.ExecuteAsync(sql);
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<IEnumerable<int>> GetExpiredAsync()
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"select Id from Alerts where EndDate < '{now}';";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      return await conn.QueryAsync<int>(sql);
    }
    finally
    {
      await conn.CloseAsync();
    }
  }
}
