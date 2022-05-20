
using JimCo.Common;
using JimCo.Models;

namespace JimCo.Services.Interfaces;
public interface IProductService : IDataService<ProductModel>
{
  Task<IEnumerable<ProductModel>> GetAsync(int pageno, int pagesize, string? categoryid = null, string columnname = "Id");
  Task<IEnumerable<ProductModel>> GetForCategoryAsync(string categoryid);
  Task<IEnumerable<ProductModel>> GetForVendorAsync(string vendorid);
  Task<IEnumerable<ProductModel>> GetForVendorAsync(string vendorid, int pageno, int pagesize = Constants.DefaultPageSize);
  Task<IEnumerable<ProductModel>> ReorderNeededAsync();
  Task<IEnumerable<ProductModel>> ReorderNeededForVendorAsync(string vendorid);
  Task<IEnumerable<ProductModel>> ReorderNeededForCategoryAsync(string categoryid);
  Task<IEnumerable<ProductModel>> SearchForProductAsync(string searchText);
  Task<IEnumerable<ProductModel>> SearchForProductAsync(string categoryid, string searchText);
  Task<IEnumerable<ProductModel>> SearchForProductBySkuAsync(string sku);
  Task<IEnumerable<ProductModel>> RandomProductsAsync(int number);
  Task<IEnumerable<ProductModel>> RandomProductsAsync(int number, IEnumerable<string> exclusionIds);
  Task<ProductModel?> ReadForSkuAsync(string sku);
  Task<ProductModel?> ReadForNameAsync(string vendorid, string name);
  Task<ApiError> UpdateCostAsync(string email, string productid, decimal cost);
  Task<ApiError> DiscontinueAsync(string email, string productid);
  Task<ApiError> SellProductsAsync(ProductSaleModel[] product);
  Task<bool> CategoryHasProductsAsync(string categoryid);
  Task<bool> VendorHasProductsAsync(string vendorid);
  Task<bool> ProductsNeedReorderAsync();
}
