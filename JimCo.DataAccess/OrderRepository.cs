using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class OrderRepository : RepositoryBase<OrderEntity>, IOrderRepository
{
  private readonly ILineItemRepository _lineItemRepository;

  public OrderRepository(IDatabase database, ILineItemRepository lineItemRepository) : base(database) => _lineItemRepository = lineItemRepository;

  private async Task FinishAsync(OrderEntity order)
  {
    if (order is not null)
    {
      order.LineItems = (await _lineItemRepository.GetForOrderAsync(order.Id)).ToList();
    }
  }

  public override async Task<IEnumerable<OrderEntity>> GetAsync(string sql, params QueryParameter[] parameters)
  {
    var parms = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var orders = await conn.QueryAsync<OrderEntity>(sql, parms);
      foreach (var order in orders)
      {
        await FinishAsync(order);
      }
      return orders;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public override async Task<OrderEntity?> ReadAsync(string sql, params QueryParameter[] parameters)
  {
    var parms = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryFirstOrDefaultAsync<OrderEntity>(sql, parms);
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

  public async Task<DalResult> CreateAsync(OrderEntity order, IEnumerable<LineItemEntity>? items = null)
  {
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction = await conn.BeginTransactionAsync();
    try
    {
      var orderid = await conn.InsertAsync(order, transaction: transaction);
      order.Id = orderid;
      if (items is not null)
      {
        foreach (var item in items)
        {
          item.OrderId = orderid;
          await conn.InsertAsync(item, transaction: transaction);
        }
      }
      await transaction.CommitAsync();
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return DalResult.FromException(ex);
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<DalResult> UpdateAsync(OrderEntity order, IEnumerable<LineItemEntity>? added = null, IEnumerable<LineItemEntity>? removed = null)
  {
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction = await conn.BeginTransactionAsync();
    try
    {
      await conn.UpdateAsync(order, transaction: transaction);
      if (removed is not null && removed.Any())
      {
        foreach (var item in removed)
        {
          await conn.DeleteAsync(item, transaction: transaction);
        }
      }
      if (added is not null && added.Any())
      {
        foreach (var item in added)
        {
          await conn.InsertAsync(item, transaction: transaction);
        }
      }
      await transaction.CommitAsync();
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return DalResult.FromException(ex);
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<IEnumerable<OrderEntity>> GetForEmailAsync(string email)
  {
    var sql = "select * from Orders where Email=@email;";
    return await GetAsync(sql, new QueryParameter { Name = "email", Value = email, Type = DbType.String});
  }

  public async Task<IEnumerable<OrderEntity>> GetForEmailAsync(string email, OrderStatus status)
  {
    var sql = $"select * from Orders where Email=@email and Status={(int)status};";
    return await GetAsync(sql, new QueryParameter {  Name = "email", Value= email, Type = DbType.String});
  }

  public async Task<IEnumerable<OrderEntity>> GetForEmailAndPinAsync(string email, int pin)
  {
    var sql = $"select * from Orders where Email=@email and Pin={pin};";
    return await GetAsync(sql, new QueryParameter { Name="email", Value= email, Type = DbType.String});
  }

  public async Task<IEnumerable<OrderEntity>> GetOpenAsync()
  {
    var sql = $"select * from Orders where Status={(int)OrderStatus.Open};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<OrderEntity>> GetPendingAsync()
  {
    var sql = $"select * from Orders where Status={(int)OrderStatus.Pending};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<OrderEntity>> GetCanceledAsync()
  {
    var sql = $"select * from Orders where Status={(int)OrderStatus.CanceledByCustomer} or Status={(int)OrderStatus.CanceledByStore};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<OrderEntity>> GetInProgressAsync()
  {
    var sql = $"select * from Orders where Status={(int)OrderStatus.InProgress};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<OrderEntity>> GetShippedAsync()
  {
    var sql = $"select * from Orders where Status={(int)OrderStatus.Shipped};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<int>> GetOpenOrderIdsAsync()
  {
    var sql = $"select id from Orders where OrderStatus={(int)OrderStatus.Open};";
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

  public async Task<DalResult> CancelOrderAsync(int orderid, bool byCustomer = true)
  {
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction = await conn.BeginTransactionAsync();
    try
    {
      var ordersql = $"select * from orders where OrderId={orderid};";
      var order = await conn.QueryFirstOrDefaultAsync<OrderEntity>(ordersql, transaction: transaction);
      if (order is null)
      {
        await transaction.RollbackAsync();
        return DalResult.NotFound;
      }
      order.Status = byCustomer ? OrderStatus.CanceledByCustomer : OrderStatus.CanceledByStore;
      order.StatusDate = DateTime.UtcNow;
      await conn.UpdateAsync(order, transaction: transaction);
      var lineitemsql = $"select * from LineItems where OrderId={orderid};";
      var lineitems = await conn.QueryAsync<LineItemEntity>(lineitemsql, transaction: transaction);
      foreach (var lineitem in lineitems)
      {
        lineitem.Status = byCustomer ? OrderStatus.CanceledByCustomer : OrderStatus.CanceledByStore;
        lineitem.StatusDate = order.StatusDate;
        await conn.UpdateAsync(lineitem, transaction: transaction);
      }
      await transaction.CommitAsync();
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return DalResult.FromException(ex);
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<DalResult> FulfillAsync(int orderid)
  {
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction= await conn.BeginTransactionAsync();
    try
    {
      var ordersql = $"select * from Orders where OrderId={orderid};";
      var order = await conn.QueryFirstOrDefaultAsync<OrderEntity>(ordersql, transaction: transaction);
      if (order is null)
      {
        await transaction.RollbackAsync();
        return DalResult.NotFound;
      }
      var shipped = true;
      var lineitemssql = $"select * from LineItems where OrderId={orderid} and Status != {(int)OrderStatus.Shipped};";
      var lineitems = await conn.QueryAsync<LineItemEntity>(lineitemssql, transaction: transaction);
      foreach (var lineitem in lineitems)
      {
        var productsql = $"select * from Products where ProductId={lineitem.ProductId};";
        var product = await conn.QueryFirstOrDefaultAsync<ProductEntity>(productsql, transaction: transaction);
        if (product is null)
        {
          lineitem.Status = OrderStatus.OutOfStock;
          lineitem.StatusDate = DateTime.UtcNow;
          shipped = false;
        }
        else if (product.Quantity < lineitem.Quantity)
        {
          lineitem.Status = OrderStatus.BackOrdered;
          lineitem.StatusDate = DateTime.UtcNow;
          shipped = false;
        }
        else
        {
          lineitem.Status = OrderStatus.Shipped;
          lineitem.StatusDate = DateTime.UtcNow;
          product.Quantity -= lineitem.Quantity;
          await conn.UpdateAsync(product, transaction: transaction);
        }
        await conn.UpdateAsync(lineitem, transaction: transaction);
      }
      if (shipped)
      {
        order.Status = OrderStatus.Shipped;
        order.StatusDate = DateTime.UtcNow;
        await conn.UpdateAsync(order, transaction: transaction);
      }
      await transaction.CommitAsync();
      return DalResult.Success;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      return DalResult.FromException(ex);
    }
    finally
    {
      await conn.CloseAsync();
    }
  }
}