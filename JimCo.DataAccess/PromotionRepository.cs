using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;

namespace JimCo.DataAccess;
public class PromotionRepository : RepositoryBase<PromotionEntity>, IPromotionRepository
{
  public PromotionRepository(IDatabase database) : base(database) { }

  public async Task<IEnumerable<PromotionEntity>> GetForProductAsync(int productid)
  {
    var sql = $"select * from Promotions where ProductId={productid};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<int>> GetCurrentIdsAsync()
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"select distinct Id from Promotions where StartDate <= '{now}' and StopDate >= '{now}';";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryAsync<int>(sql);
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<IEnumerable<PromotionEntity>> GetCurrentForProductAsync(int productid)
  {
    var now = DateTime.UtcNow.ToString(Constants.DateFormat);
    var sql = $"select * from Promotions where ProductId={productid} and StartDate <= '{now}' and StopDate >= '{now}';";
    return await GetAsync(sql);
  }

  public async Task<int> GetProductIdAsync(int promotionid)
  {
    var sql = $"select ProductId from Promotions where Id={promotionid};";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.ExecuteScalarAsync<int>(sql);
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<bool> ProductHasPromotionsAsync(int productid)
  {
    var sql = $"select count(*) from Promotions where ProductId={productid};";
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var count = await conn.ExecuteScalarAsync<int>(sql);
      return count > 0;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }
}
