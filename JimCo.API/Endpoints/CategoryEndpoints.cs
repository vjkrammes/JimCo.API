using JimCo.Common;
using JimCo.Services.Interfaces;

using Zxcvbn;

namespace JimCo.API.Endpoints;

public static class CategoryEndpoints
{
  public static void ConfigureCategoryEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Category", GetCategories);
    app.MapGet("/api/v1/Category/ById/{categoryid}", ById);
    app.MapGet("/api/v1/Category/ByName/{categoryname}", ByName);
  }

  private static async Task<IResult> GetCategories(ICategoryService categoryService) => 
    Results.Ok((await categoryService.GetAsync()).OrderBy(x => x.Name));

  private static async Task<IResult> ById(string categoryid, ICategoryService categoryService)
  {
    var category = await categoryService.ReadAsync(categoryid);
    return category is null 
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "category", "id", categoryid))) 
      : Results.Ok(category);
  }

  private static async Task<IResult> ByName(string categoryname, ICategoryService categoryService)
  {
    var category = await categoryService.ReadByNameAsync(categoryname);
    return category is null
      ? Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "category", "name", categoryname)))
      : Results.Ok(category);
  }
}
