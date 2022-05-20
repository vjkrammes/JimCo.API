using System.Data;
using System.Data.SqlClient;

using Dapper;
using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Newtonsoft.Json;

namespace JimCo.DataAccess;
public class AlertRepository : RepositoryBase<AlertEntity>, IAlertRepository
{
  private readonly IUserRepository _userRepository;

  public AlertRepository(IDatabase database, IUserRepository userRepository) : base(database) => _userRepository = userRepository;

  private static IEnumerable<AlertEntity> FilterForRoles(IEnumerable<AlertEntity> alerts, string[] rolenames)
  {
    var ret = new List<AlertEntity>();
    foreach (var alert in alerts)
    {
      var roles = JsonConvert.DeserializeObject<List<string>>(alert.Roles) ?? new();
      foreach (var rolename in rolenames)
      {
        if (roles.Contains(rolename) && !ret.Contains(alert))
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

  public async Task<IEnumerable<AlertEntity>> GetForIdentifierAsync(string identifier, bool includeAcknowedged = false)
  {
    string sql = !includeAcknowedged
      ? "select * from Alerts where Identifier=@Identifier and Acknowledged=0;"
      : "select * from Alerts where Identifier=@Identifier;";
    return await GetAsync(sql, new QueryParameter { Name = "Identifier", Value = identifier, Type = DbType.String });
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

  public async Task<DalResult> Acknowledge(AlertEntity alert)
  {
    if (alert is null)
    {
      return new(DalErrorCode.InvalidParameters, new Exception("Alert is required"));
    }
    if (!alert.RequiresAcknowledgement)
    {
      return DalResult.Success;
    }
    if (string.IsNullOrWhiteSpace(alert.Identifier))
    {
      return DalResult.Success;
    }
    var recipient = await _userRepository.ReadForIdentifierAsync(alert.Identifier);
    if (recipient is null)
    {
      return DalResult.NotFound;
    }
    var sender = await _userRepository.ReadAsync(alert.Creator);
    if (sender is null)
    {
      return DalResult.NotFound;
    }
    if (await ReadAsync(alert.Id) is null)
    {
      return DalResult.NotFound;
    }
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction = await conn.BeginTransactionAsync();
    try
    {
      alert.Acknowledged = true;
      alert.AcknowledgedOn = DateTime.UtcNow;
      await conn.UpdateAsync(alert, transaction);
      var newalert = new AlertEntity
      {
        Id = 0,
        Level = AlertLevel.Information,
        Roles = "",
        Identifier = sender.Identifier,
        Title = "Alert Acknowledged",
        Text = $"Your Alert with the title '{alert.Title}' sent to {recipient} was acknowledged on {alert.AcknowledgedOn.ToShortDateString()}",
        CreateDate = DateTime.UtcNow,
        Creator = recipient.Email,
        StartDate = default,
        EndDate = default,
        RequiresAcknowledgement = false,
        Acknowledged = false,
        AcknowledgedOn = default
      };
      await conn.InsertAsync(newalert, transaction);
      await transaction.CommitAsync();
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return new DalResult(DalErrorCode.Exception, ex);
    }
  }

  public async Task<int> DeleteExpiredAsync()
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"delete from Alerts where EndDate < '{now}' and Identifier = '';";
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

  public async Task<DalResult> DeleteAllAsync(string identifier)
  {
    if (string.IsNullOrWhiteSpace(identifier))
    {
      return DalResult.NotFound;
    }
    var sql = "delete from Alerts where Identifier=@identifier and (RequiresAcknowledgement = 0 or Acknowledged = 1);";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      await conn.ExecuteAsync(sql, new { Identifier = identifier });
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      return DalResult.FromException(ex);
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
