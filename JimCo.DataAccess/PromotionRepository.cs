using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess;
public class PromotionRepository : RepositoryBase<PromotionEntity>, IPromotionRepository
{
  private readonly IUserRepository _userRepository;

  public PromotionRepository(IDatabase database, IUserRepository userRepository) : base(database) => _userRepository = userRepository;

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

  public async Task<DalResult> CancelAsync(int promotionId, string canceledBy)
  {
    if (promotionId <= 0)
    {
      return DalResult.NotFound;
    }
    if (string.IsNullOrWhiteSpace(canceledBy))
    {
      return new(DalErrorCode.NotAuthenticated, new Exception("Identifier of person cancelling the promotion is required"));
    }
    var user = await _userRepository.ReadForIdentifierAsync(canceledBy);
    if (user is null)
    {
      return new(DalErrorCode.NotFound, new Exception("Identifier not found"));
    }
    var roles = JsonConvert.DeserializeObject<string[]>(user.JobTitles);
    if (roles is null || !roles.Any())
    {
      return DalResult.NotAuthorized;
    }
    if (!(roles.Contains("manager", StringComparer.OrdinalIgnoreCase) || roles.Contains("admin", StringComparer.OrdinalIgnoreCase)))
    {
      return DalResult.NotAuthorized;
    }
    var sql = $"Update Promotions set CanceledOn=@canceledon, CanceledBy=@canceledby where Id={promotionId};";
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    try
    {
      var response = await conn.ExecuteAsync(sql, new { CanceledOn = DateTime.UtcNow, CanceledBy = user.Email});
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

  public async Task<DalResult> UnCancelAsync(int promotionId)
  {
    if (promotionId <= 0)
    {
      return DalResult.NotFound;
    }
    var sql = $"Update Promotions set CanceledOn='0001-01-01', CanceledBy='' where Id={promotionId};";
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    try
    {
      var response = await conn.ExecuteAsync(sql);
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

  public async Task<DalResult> DeleteAllExpiredAsync()
  {
    var sql = "delete from Promotions where YEAR(CanceledOn) != 1 or StopDate < @currentdate;";
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    try
    {
      await conn.ExecuteAsync(sql, new { currentdate = DateTime.UtcNow });
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

  public async Task<DalResult> DeleteExpiredAsync(int productId)
  {
    var sql = $"delete from Promotions where ProductId={productId} and (YEAR(CanceledOn) != 1 or StopDate < @currentdate);";
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    try
    {
      await conn.ExecuteAsync(sql, new { currentdate = DateTime.UtcNow });
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
}
