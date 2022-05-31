using JimCo.Common;
using JimCo.Common.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

using Microsoft.Extensions.Options;

namespace JimCo.API.Endpoints;

public static class CategoryEndpoints
{
  public static void ConfigureCategoryEndpoints(this WebApplication app)
  {
    app.MapGet("/api/v1/Category", GetCategories);
    app.MapGet("/api/v1/Category/ById/{categoryid}", ById);
    app.MapGet("/api/v1/Category/ByName/{categoryname}", ByName);
    app.MapPost("/api/v1/Category", Create).RequireAuthorization("ManagerPlusRequired");
    app.MapPut("/api/v1/Category", Update).RequireAuthorization("ManagerPlusRequired");
    app.MapDelete("/api/v1/Category/Delete/{categoryid}", Delete).RequireAuthorization("ManagerPlusRequired");
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

  private static async Task<IResult> Create(CategoryModel model, ICategoryService categoryService, IOptions<AppSettings> settings, IUriHelper helper)
  {
    if (settings is null)
    {
      throw new ArgumentNullException(nameof(settings));
    }
    if (helper is null)
    {
      throw new ArgumentNullException(nameof(helper));
    }
    helper.SetBase(settings.Value.ApiBase);
    helper.SetVersion(1);
    if (model is null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Image))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    if (string.IsNullOrWhiteSpace(model.Id))
    {
      model.Id = IdEncoder.EncodeId(0);
    }
    model.Name = model.Name.Capitalize();
    if (string.IsNullOrWhiteSpace(model.Background))
    {
      model.Background = string.IsNullOrEmpty(settings.Value.DefaultBackground) ? "White" : settings.Value.DefaultBackground;
    }
    if (model.AgeRequired < 0)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Invalid, "age required")));
    }
    if ((model.IsAgeRestricted && model.AgeRequired == 0) || (!model.IsAgeRestricted && model.AgeRequired != 0))
    {
      return Results.BadRequest(new ApiError(Strings.BadAge));
    }
    var result = await categoryService.InsertAsync(model);
    if (result.Successful)
    {
      return Results.Created(helper.Create("Category", "ById", model.Id), model);
    }
    return Results.BadRequest(result);
  }

  private static async Task<IResult> Update(CategoryModel model, ICategoryService categoryService, IOptions<AppSettings> settings)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Image))
    {
      return Results.BadRequest(new ApiError(Strings.InvalidModel));
    }
    var result = await categoryService.UpdateAsync(model);
    if (result.Successful)
    {
      return Results.Ok(model);
    }
    return Results.BadRequest(result);
  }
  
  private static async Task<IResult> Delete(string categoryid, ICategoryService categoryService)
  {
    if (string.IsNullOrWhiteSpace(categoryid))
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.Required, "id")));
    }
    var model = await categoryService.ReadAsync(categoryid);
    if (model is null)
    {
      return Results.BadRequest(new ApiError(string.Format(Strings.NotFound, "category", "id", categoryid)));
    }
    var result = await categoryService.DeleteAsync(model);
    if (result.Successful)
    {
      return Results.Ok();
    }
    return Results.BadRequest(result);
  }
}
