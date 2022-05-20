
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class ProductService : IProductService
{
  private readonly IProductRepository _productRepository;
  private readonly ICategoryRepository _categoryRepository;
  private readonly IVendorRepository _vendorRepository;
  private readonly ILineItemRepository _lineItemRepository;
  private readonly IPromotionRepository _promotionRepository;

  public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IVendorRepository vendorRepository,
    ILineItemRepository lineItemRepository, IPromotionRepository promotionRepository)
  {
    _productRepository = productRepository;
    _categoryRepository = categoryRepository;
    _vendorRepository = vendorRepository;
    _lineItemRepository = lineItemRepository;
    _promotionRepository = promotionRepository;
  }

  public async Task<int> CountAsync() => await _productRepository.CountAsync();

  public async Task<ApiError> ValidateModelAsync(ProductModel model, bool checkid = false, bool update = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.CategoryId) || string.IsNullOrWhiteSpace(model.VendorId) || string.IsNullOrWhiteSpace(model.Name)
      || string.IsNullOrWhiteSpace(model.Description) || string.IsNullOrWhiteSpace(model.Sku) || model.Price <= 0M || model.AgeRequired < 0
      || model.Quantity < 0 || model.ReorderLevel < 0 || model.ReorderAmount < 0 || model.Cost < 0M)
    {
      return new(Strings.InvalidModel);
    }
    var catid = IdEncoder.DecodeId(model.CategoryId);
    if (catid <= 0 || await _categoryRepository.ReadAsync(catid) is null)
    {
      return new(string.Format(Strings.NotFound, "category", "id", model.CategoryId));
    }
    var vendorid = IdEncoder.DecodeId(model.VendorId);
    if (vendorid <= 0 || await _vendorRepository.ReadAsync(vendorid) is null)
    {
      return new(string.Format(Strings.NotFound, "vendor", "id", model.VendorId));
    }
    if (model.Promotions is null)
    {
      model.Promotions = new();
    }
    if (model.Cost > model.Price)
    {
      return new(string.Format(Strings.Invalid, "cost"));
    }
    if (string.IsNullOrWhiteSpace(model.Id))
    {
      model.Id = IdEncoder.EncodeId(0);
    }
    if (checkid)
    {
      var decodedid = IdEncoder.DecodeId(model.Id);
      if (decodedid <= 0)
      {
        return new(string.Format(Strings.Invalid, "id"));
      }
    }
    var existing = await _productRepository.ReadForNameAsync(vendorid, model.Name);
    if (update)
    {
      if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
      {
        return new(string.Format(Strings.DuplicateProduct, model.VendorId, model.Name));
      }
    }
    else if (existing is not null)
    {
      return new(string.Format(Strings.DuplicateProduct, model.VendorId, model.Name));
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(ProductModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    ProductEntity entity = model!;
    try
    {
      var result = await _productRepository.InsertAsync(entity);
      if (result.Successful)
      {
        model.Id = IdEncoder.EncodeId(entity.Id);
      }
      return ApiError.FromDalResult(result);
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> UpdateAsync(ProductModel model)
  {
    var checkresult = await ValidateModelAsync(model, true, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    ProductEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _productRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(ProductModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    if (!model.CanDelete)
    {
      return new(string.Format(Strings.CantDelete, "product", "items"));
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "product", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _productRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private async Task<bool> ProductCanBeDeleted(ProductModel model)
  {
    var id = IdEncoder.DecodeId(model.Id);
    return !(await _promotionRepository.ProductHasPromotionsAsync(id) || await _lineItemRepository.ProductHasLineItemsAsync(id));
  }

  private async Task<IEnumerable<ProductModel>> Finish(IEnumerable<ProductEntity> entities)
  {
    var models = entities.ToModels<ProductModel, ProductEntity>();
    foreach (var model in models)
    {
      model.CanDelete = await ProductCanBeDeleted(model);
    }
    return models;
  }

  public async Task<IEnumerable<ProductModel>> GetAsync()
  {
    var entities = await _productRepository.GetAsync();
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> GetAsync(int pageno, int pagesize, string? categoryid = null, string columnname = "Id")
  {
    var catid = string.IsNullOrWhiteSpace(categoryid) ? 0 : IdEncoder.DecodeId(categoryid);
    var entities = await _productRepository.GetAsync(pageno, pagesize, catid, columnname);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> GetForCategoryAsync(string categoryid)
  {
    var catid = IdEncoder.DecodeId(categoryid);
    var entities = await _productRepository.GetForCategoryAsync(catid);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> GetForVendorAsync(string vendorid)
  {
    var vid = IdEncoder.DecodeId(vendorid);
    var entities = await _productRepository.GetForVendorAsync(vid);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> GetForVendorAsync(string vendorid, int pagneno, int pagesize = Constants.DefaultPageSize)
  {
    var vid = IdEncoder.DecodeId(vendorid);
    var entities = await _productRepository.GetForVendorAsync(vid, pagneno, pagesize);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> ReorderNeededAsync()
  {
    var entities = await _productRepository.ReorderNeededAsync();
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> ReorderNeededForVendorAsync(string vendorid)
  {
    var vid = IdEncoder.DecodeId(vendorid);
    var entities = await _productRepository.ReorderNeededForVendorAsync(vid);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> ReorderNeededForCategoryAsync(string categoryid)
  {
    var catid = IdEncoder.DecodeId(categoryid);
    var entities = await _productRepository.ReorderNeededForCategoryAsync(catid);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> SearchForProductAsync(string searchText)
  {
    var entities = await _productRepository.SearchForProductAsync(searchText);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> SearchForProductAsync(string categoryid, string searchText)
  {
    var catid = IdEncoder.DecodeId(categoryid);
    var entities = await _productRepository.SearchForProductAsync(catid, searchText);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> SearchForProductBySkuAsync(string sku)
  {
    var entities = await _productRepository.SearchForProductBySkuAsync(sku);
    return await Finish(entities);
  }

  public async Task<IEnumerable<ProductModel>> RandomProductsAsync(int number) => await RandomProductsAsync(number, Array.Empty<string>());

  public async Task<IEnumerable<ProductModel>> RandomProductsAsync(int number, IEnumerable<string> exclusionIds)
  {
    var exclusions = exclusionIds.Select(x => IdEncoder.DecodeId(x));
    var entities = await _productRepository.RandomProductsAsync(number, exclusions);
    return await Finish(entities);
  }

  private async Task<ProductModel?> Finish(ProductEntity? entity)
  {
    ProductModel ret = entity!;
    if (ret is not null)
    {
      ret.CanDelete = await ProductCanBeDeleted(ret);
    }
    return ret;
  }

  public async Task<ProductModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    var entity = await _productRepository.ReadAsync(decodedid);
    return await Finish(entity);
  }

  public async Task<ProductModel?> ReadForSkuAsync(string sku) => await Finish(await _productRepository.ReadForSkuAsync(sku));

  public async Task<ProductModel?> ReadForNameAsync(string vendorid, string name)
  {
    var vid = IdEncoder.DecodeId(vendorid);
    return await Finish(await _productRepository.ReadForNameAsync(vid, name));
  }

  public async Task<ApiError> UpdateCostAsync(string email, string productid, decimal cost)
  {
    var pid = IdEncoder.DecodeId(productid);
    return ApiError.FromDalResult(await _productRepository.UpdateCostAsync(email, pid, cost));
  }

  public async Task<ApiError> DiscontinueAsync(string email, string productid)
  {
    var pid = IdEncoder.DecodeId(productid);
    return ApiError.FromDalResult(await _productRepository.DiscontinueAsync(email, pid));
  }

  public async Task<ApiError> SellProductsAsync(ProductSaleModel[] products) =>
    ApiError.FromDalResult(await _productRepository.SellProductsAsync(products.Select(x =>
    new ProductSaleEntity { ProductId = IdEncoder.DecodeId(x.ProductId), Quantity = x.Quantity }).ToArray()));

  public async Task<bool> CategoryHasProductsAsync(string categoryid)
  {
    var catid = IdEncoder.DecodeId(categoryid);
    return await _productRepository.CategoryHasProductsAsync(catid);
  }

  public async Task<bool> VendorHasProductsAsync(string vendorid)
  {
    var vid = IdEncoder.DecodeId(vendorid);
    return await _productRepository.VendorHasProductsAsync(vid);
  }

  public async Task<bool> ProductsNeedReorderAsync() => await _productRepository.ProductsNeedReorderAsync();
}
