using System.Data.SqlClient;

using Dapper;

using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class LineItemRepository : RepositoryBase<LineItemEntity>, ILineItemRepository
{
  private readonly IProductRepository _productRepository;

  public LineItemRepository(IDatabase database, IProductRepository productRepository) : base(database) => _productRepository = productRepository;

  private async Task FinishAsync(LineItemEntity entity)
  {
    if (entity is not null)
    {
      entity.Product = await _productRepository.ReadAsync(entity.ProductId)!;
    }
  }

  public override async Task<IEnumerable<LineItemEntity>> GetAsync(string sql, params QueryParameter[] parameters)
  {
    var parm = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryAsync<LineItemEntity>(sql, parm);
      foreach (var item in ret)
      {
        await FinishAsync(item);
      }
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public override async Task<LineItemEntity?> ReadAsync(string sql, params QueryParameter[] parameters)
  {
    var parm = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryFirstOrDefaultAsync<LineItemEntity>(sql, parm);
      if (ret is not null)
      {
        await FinishAsync(ret);
      }
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<IEnumerable<LineItemEntity>> GetForOrderAsync(int orderid)
  {
    var sql = $"select * from LineItems where OrderId={orderid};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<LineItemEntity>> GetForProductAsync(int productid)
  {
    var sql = $"select * from LineItems where ProductId={productid};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<LineItemEntity>> GetUnderstockedAsync()
  {
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      var sql = "select distinct Id from Products where Quantity <= ReorderLevel;";
      var ids = await conn.QueryAsync<int>(sql);
      List<LineItemEntity> ret = new();
      foreach (var id in ids)
      {
        var lineitems = await GetForProductAsync(id);
        ret.AddRange(lineitems);
      }
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  private async Task<bool> NonZeroCount(string sql)
  {
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.ExecuteScalarAsync<int>(sql);
      return ret > 0;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<bool> OrderHasLineItemsAsync(int orderid)
  {
    var sql = $"select count(*) from LineItems where OrderId={orderid};";
    return await NonZeroCount(sql);
  }

  public async Task<bool> ProductHasLineItemsAsync(int productid)
  {
    var sql = $"select count(*) from LineItems where ProductId={productid};";
    return await NonZeroCount(sql);
  }
}
