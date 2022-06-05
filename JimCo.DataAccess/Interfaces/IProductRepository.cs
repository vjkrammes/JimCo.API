
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.DataAccess.Interfaces;
public interface IProductRepository : IRepository<ProductEntity>
{
  Task<IEnumerable<ProductEntity>> GetAsync(int pageno, int pagesize, int categoryid = 0, string columnname = "Id");
  Task<IEnumerable<ProductEntity>> GetForCategoryAsync(int categoryid);
  Task<IEnumerable<ProductEntity>> GetForVendorAsync(int vendorid);
  Task<IEnumerable<ProductEntity>> GetForVendorAsync(int vendorid, int pageno, int pagesize, string columnname = "Id");
  Task<IEnumerable<ProductEntity>> ReorderNeededAsync();
  Task<IEnumerable<ProductEntity>> ReorderNeededForVendorAsync(int vendorid);
  Task<IEnumerable<ProductEntity>> ReorderNeededForCategoryAsync(int categoryid);
  Task<IEnumerable<ProductEntity>> SearchForProductAsync(string searchText);
  Task<IEnumerable<ProductEntity>> SearchForProductAsync(int categoryid, string searchText);
  Task<IEnumerable<ProductEntity>> SearchForProductBySkuAsync(string sku);
  Task<IEnumerable<ProductEntity>> RandomProductsAsync(int number, IEnumerable<int> exclusions);
  Task<ProductEntity?> ReadForSkuAsync(string sku);
  Task<ProductEntity?> ReadForNameAsync(int vendorid, string name);
  Task<DalResult> UpdateCostAsync(string email, int productid, decimal cost);
  Task<DalResult> DiscontinueAsync(string email, int productid);
  Task<DalResult> VendorUpdateAsync(int id, int reorderAmount, decimal cost);
  Task<DalResult> SellProductsAsync(ProductSaleEntity[] entities);
  Task<bool> CategoryHasProductsAsync(int categoryid);
  Task<bool> VendorHasProductsAsync(int vendorid);
  Task<bool> ProductsNeedReorderAsync();
}
