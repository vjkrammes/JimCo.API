using System.Data;
using System.Data.SqlClient;

using Dapper;
using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

namespace JimCo.DataAccess;
public class ProductRepository : RepositoryBase<ProductEntity>, IProductRepository
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly IPromotionRepository _promotionRepository;
  private readonly IVendorRepository _vendorRepository;

  public ProductRepository(IDatabase database, ICategoryRepository categoryRepository, IPromotionRepository promotionRepository,
    IVendorRepository vendorRepository) : base(database)
  {
    _categoryRepository = categoryRepository;
    _promotionRepository = promotionRepository;
    _vendorRepository = vendorRepository;
  }

  private async Task Finish(ProductEntity entity)
  {
    if (entity is not null)
    {
      entity.Category = await _categoryRepository.ReadAsync(entity.CategoryId);
      entity.Promotions = (await _promotionRepository.GetCurrentForProductAsync(entity.Id)).ToList();
      entity.Vendor = await _vendorRepository.ReadAsync(entity.VendorId);
    }
  }

  public override async Task<IEnumerable<ProductEntity>> GetAsync(string sql, params QueryParameter[] parameters)
  {
    var parm = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryAsync<ProductEntity>(sql, parm);
      foreach (var item in ret)
      {
        await Finish(item);
      }
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public override async Task<ProductEntity?> ReadAsync(string sql, params QueryParameter[] parameters)
  {
    var parm = BuildParameters(parameters);
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var ret = await conn.QueryFirstOrDefaultAsync<ProductEntity>(sql, parm);
      if (ret is not null)
      {
        await Finish(ret);
      }
      return ret;
    }
    finally
    {
      await conn.CloseAsync();
    }
  }

  public async Task<IEnumerable<ProductEntity>> GetAsync(int pageno, int pagesize, int categoryid = 0, string columnname = "Id")
  {
    if (pageno < 0)
    {
      pageno = 0;
    }
    if (pagesize <= 0)
    {
      pagesize = Constants.DefaultPageSize;
    }
    var offset = pageno * pagesize;
    string sql = categoryid == 0
      ? $"select * from Products order by {columnname} offset {offset} rows fetch next {pagesize} rows only;"
      : $"select * from Products where CategoryId={categoryid} order by {columnname} offset {offset} rows fetch next {pagesize} rows only;";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> GetForCategoryAsync(int categoryid)
  {
    var sql = $"select * from Products where CategoryId={categoryid};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> GetForVendorAsync(int vendorid)
  {
    var sql = $"select * from Products where VendorId={vendorid};";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> GetForVendorAsync(int vendorid, int pageno, int pagesize, string columnname = "Id")
  {
    if (pageno < 0)
    {
      pageno = 0;
    }
    if (pagesize <= 0)
    {
      pagesize = Constants.DefaultPageSize;
    }
    var offset = pageno * pagesize;
    var sql = $"select * from Products where VendorId={vendorid} order by {columnname} offset {offset} rows fetch next {pagesize} rows only;";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> ReorderNeededAsync()
  {
    var sql = "select * from Products where Quantity <= ReorderLevel;";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> ReorderNeededForVendorAsync(int vendorid)
  {
    var sql = $"select * from Products where VendorId={vendorid} and Quantity <= ReorderLevel;";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> ReorderNeededForCategoryAsync(int categoryid)
  {
    var sql = $"select * from Products where CategoryId={categoryid} and Quantity <= ReorderLevel;";
    return await GetAsync(sql);
  }

  public async Task<IEnumerable<ProductEntity>> SearchForProductAsync(string searchText)
  {
    var sql = "select * from Products where Name like CONCAT('%', @searchText, '%') or Description like CONCAT('%', @searchText, '%');";
    return await GetAsync(sql, new QueryParameter("searchText", searchText, DbType.String));
  }

  public async Task<IEnumerable<ProductEntity>> SearchForProductAsync(int categoryid, string searchText)
  {
    var sql = $"select * from Products where CategoryId={categoryid} and (Name like CONCAT('%', @searchText, '%') or Description like CONCAT('%', @searchText, '%'));";
    return await GetAsync(sql, new QueryParameter("searchText", searchText, DbType.String));
  }

  public async Task<IEnumerable<ProductEntity>> SearchForProductBySkuAsync(string sku)
  {
    var sql = "select * from Products where Sku like CONCAT('%', @sku, '%');";
    return await GetAsync(sql, new QueryParameter("sku", sku, DbType.String));
  }

  private async Task<ProductEntity> RandomProductAsync()
  {
    var sql = "select top 1 * from products order by NEWID();";
    var entity = await ReadAsync(sql);
    return entity!;
  }

  public async Task<IEnumerable<ProductEntity>> RandomProductsAsync(int number, IEnumerable<int> exclusions)
  {
    List<ProductEntity> ret = new();
    if (number <= 0)
    {
      return ret;
    }
    List<int> ids = new();
    List<int> excluded = exclusions.ToList();
    var ix = 0;
    while (ix < number)
    {
      var entity = await RandomProductAsync();
      var iterations = 0;
      while (ids.Contains(entity.Id) || excluded.Contains(entity.Id))
      {
        if (++iterations > 10)
        {
          break; // give up, there may be duplicate items or excluded items
        }
        entity = await RandomProductAsync();
      }
      ret.Add(entity);
      ids.Add(entity.Id);
      ix++;
    }
    return ret;
  }

  public async Task<ProductEntity?> ReadForSkuAsync(string sku)
  {
    var sql = "select * from Products where Sku=@sku;";
    return await ReadAsync(sql, new QueryParameter("sku", sku, DbType.String));
  }

  public async Task<ProductEntity?> ReadForNameAsync(int vendorid, string name)
  {
    var sql = $"select * from Products where VendorId={vendorid} and Name=@name;";
    return await ReadAsync(sql, new QueryParameter("name", name, DbType.String));
  }

  private async Task<bool> NonZeroCountAsync(string sql)
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

  public async Task<DalResult> UpdateCostAsync(string email, int productid, decimal cost)
  {
    if (cost <= 0M)
    {
      return new(DalErrorCode.InvalidParameters, new Exception("Cost must be greater than zero"));
    }
    using var conn = new SqlConnection(ConnectionString);
    try
    {
      await conn.OpenAsync();
      var vendor = await _vendorRepository.ReadAsync(email);
      if (vendor is null)
      {
        return new(DalErrorCode.NotFound, new Exception("Vendor not found"));
      }
      var product = await ReadAsync(productid);
      if (product is null)
      {
        return new(DalErrorCode.NotFound, new Exception("Product not found"));
      }
      if (product.VendorId != vendor.Id)
      {
        return new(DalErrorCode.NotAuthorized, new Exception("Requester cannot update that product"));
      }
      product.Cost = cost;
      await conn.UpdateAsync(product);
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

  public async Task<DalResult> DiscontinueAsync(string email, int productid)
  {
    var vendor = await _vendorRepository.ReadForEmailAsync(email);
    if (vendor is null)
    {
      return new(DalErrorCode.NotFound, new Exception("Vendor not found"));
    }
    var product = await ReadAsync(productid);
    if (product is null)
    {
      return new(DalErrorCode.NotFound, new Exception("Product not found"));
    }
    if (product.VendorId != vendor.Id)
    {
      return new(DalErrorCode.NotAuthorized, new Exception("Requester cannot update that product"));
    }
    using var conn = new SqlConnection(ConnectionString);
    product.Discontinued = !product.Discontinued;
    try
    {
      await conn.OpenAsync();
      await conn.UpdateAsync(product);
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

  public async Task<DalResult> VendorUpdateAsync(int id, int reorderAmount, decimal cost)
  {
    var product = await ReadAsync(id);
    if (product is null)
    {
      return DalResult.Duplicate;
    }
    product.ReorderAmount = reorderAmount;
    product.Cost = cost;
    return await UpdateAsync(product);
  }

  public async Task<DalResult> SellProductsAsync(ProductSaleEntity[] entities)
  {
    if (entities is null || !entities.Any())
    {
      return DalResult.NotFound;
    }
    using var conn = new SqlConnection(ConnectionString);
    await conn.OpenAsync();
    using var transaction = await conn.BeginTransactionAsync();
    try
    {
      foreach (var entity in entities)
      {
        var product = await ReadAsync(entity.ProductId);
        if (product is null)
        {
          await transaction.RollbackAsync();
          return new(DalErrorCode.NotFound, new Exception($"No product with the id '{entity.ProductId}' was found"));
        }
        product.Quantity -= entity.Quantity;
        if (product.Quantity < 0)
        {
          product.Quantity = 0;
        }
        var result = await UpdateAsync(product);
        if (!result.Successful)
        {
          await transaction.RollbackAsync();
          return result;
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

  public async Task<bool> CategoryHasProductsAsync(int categoryid)
  {
    var sql = $"select count(*) from Products where CategoryId={categoryid};";
    return await NonZeroCountAsync(sql);
  }

  public async Task<bool> VendorHasProductsAsync(int vendorid)
  {
    var sql = $"select count(*) from Products where VendorId={vendorid};";
    return await NonZeroCountAsync(sql);
  }

  public async Task<bool> ProductsNeedReorderAsync()
  {
    var sql = $"select count(*) from Products where Quantity <= ReorderLevel;";
    return await NonZeroCountAsync(sql);
  }
}
