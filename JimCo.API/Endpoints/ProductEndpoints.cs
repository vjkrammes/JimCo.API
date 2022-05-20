using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class ProductEndpoints
{
  public static void ConfigureProductEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Product", GetProducts);
    app.MapGet("/api/v1/Product/Search/{searchText}", SearchProducts);
    app.MapGet("/api/v1/Product/Search/{categoryId}/{searchText}", SearchProductsWithCategory);
    app.MapGet("/api/v1/Product/ById/{productid}", ById);
    app.MapGet("/api/v1/Product/ByName/{vendorid}/{name}", ByName);
    app.MapGet("/api/v1/Product/BySku/{sku}", BySku);
    app.MapGet("/api/v1/Product/ForCategory/{categoryId}", ForCategory);
    app.MapGet("/api/v1/Product/Random/{count}", Random);
    app.MapPost("/api/v1/Product", Create).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Product", Update).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Product/Delete/{id}", Delete).RequireAuthorization("ManagerPlusRequired");
  }

  private static async Task<IResult> GetProducts(IProductService productService) => Results.Ok(await productService.GetAsync());

  private static async Task<IResult> SearchProducts(string searchText, IProductService productService)
  {
    var result = await productService.SearchForProductAsync(searchText);
    return Results.Ok(result);
  }

  private static async Task<IResult> SearchProductsWithCategory(string categoryId, string searchText, IProductService productService)
  {
    var result = await productService.SearchForProductAsync(categoryId, searchText);
    if (string.IsNullOrWhiteSpace(categoryId) || categoryId == "0")
    {
      return await SearchProducts(searchText, productService);
    }
    return Results.Ok(result);
  }

  private static async Task<IResult> ById(string productid, IProductService productService)
  {
    var product = await productService.ReadAsync(productid);
    return product is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.ProductNotFound, "id", productid)))
      : Results.Ok(product);
  }

  private static async Task<IResult> ByName(string vendorid, string name, IProductService productService)
  {
    var product = await productService.ReadForNameAsync(vendorid, name);
    return product is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.ProductNotFound, "name", name)))
      : Results.Ok(product);
  }

  private static async Task<IResult> BySku(string sku, IProductService productService)
  {
    var product = await productService.ReadForSkuAsync(sku);
    return product is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.ProductNotFound, "SKU", sku)))
      : Results.Ok(product);
  }

  private static async Task<IResult> ForCategory(string categoryid, IProductService productService)
  {
    var products = await productService.GetForCategoryAsync(categoryid);
    return Results.Ok(products);
  }

  private static async Task<IResult> Random(int count, IProductService productService)
  {
    var products = await productService.RandomProductsAsync(count);
    return Results.Ok(products);
  }

  private static async Task<IResult> Create([FromBody] ProductModel model, IProductService productService, IVendorService vendorService, 
    ICategoryService categoryService, IUriHelper helper, IOptions<AppSettings> settings)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Description) || string.IsNullOrWhiteSpace(model.Sku) ||
      model.Price <= 0M)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var category = await categoryService.ReadAsync(model.CategoryId);
    if (category is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "category", "id", model.CategoryId)));
    }
    var vendor = await vendorService.ReadAsync(model.VendorId);
    if (vendor is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "vendor", "id", model.VendorId)));
    }
    var result = await productService.InsertAsync(model);
    if (result.Successful)
    {
      helper.SetBase(settings.Value.ApiBase);
      helper.SetVersion(1);
      var uri = helper.Create("Product", "ById", model.Id);
      return Results.Created(uri.ToString(), model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> Update([FromBody] ProductModel model, IProductService productService)
  {
    if (model is null)
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var result = await productService.UpdateAsync(model);
    if (result.Successful)
    {
      return Results.Ok(model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> Delete(string id, IProductService productService)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "id")));
    }
    var product = await productService.ReadAsync(id);
    if (product is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "product", "id", id)));
    }
    var result = await productService.DeleteAsync(product);
    if (result.Successful)
    {
      return Results.NoContent();
    }
    return Results.BadRequest(result);
  }
}
