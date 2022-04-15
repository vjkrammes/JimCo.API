using JimCo.Common;
using JimCo.Services.Interfaces;

namespace JimCo.API.Endpoints;

public static class ProductEndpoints
{
  public static void ConfigureProductEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Product", GetProducts);
    app.MapGet("/api/v1/Product/Search/{searchText}", SearchProducts);
    app.MapGet("/api/v1/Product/ById/{productid}", ById);
    app.MapGet("/api/v1/Product/ByName/{vendorid}/{name}", ByName);
    app.MapGet("/api/v1/Product/ForCategory/{categoryId}", ForCategory);
    app.MapGet("/api/v1/Product/Random/{count}", Random);
  }

  private static async Task<IResult> GetProducts(IProductService productService) => Results.Ok(await productService.GetAsync());

  private static async Task<IResult> SearchProducts(string searchText, IProductService productService)
  {
    var result = await productService.SearchForProductAsync(searchText);
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
}
