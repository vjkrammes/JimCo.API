using System.Data;
using System.Data.SqlClient;

using Dapper;

using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class LogRepository : RepositoryBase<LogEntity>, ILogRepository
{
  public LogRepository(IDatabase database) : base(database) { }

  public async Task<IEnumerable<LogEntity>> GetForDateAsync(DateTime date, Level level = Level.NoLevel)
  {
    string sql = level == Level.NoLevel
      ? "select * from  Logs where CAST(Timestamp as DATE) = CAST(@date as DATE) order by Timestamp desc;"
      : $"select * from Logs where CAST(Timestamp as DATE) = CAST(@date as DATE) and Level={(int)level} by Timestamp desc;";
    return await GetAsync(sql, new QueryParameter("date", date, DbType.Date));
  }

  public async Task<IEnumerable<DateTime>> GetDatesAsync()
  {
    var sql = "select l.[date] from (select distinct CAST(Timestamp as DATE) as [date] from Logs) as l;";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryAsync<DateTime>(sql);
      if (ret is not null)
      {
        return ret;
      }
      return Array.Empty<DateTime>();
    }
    finally
    {
      await conn.CloseAsync();
    }
  }
}
