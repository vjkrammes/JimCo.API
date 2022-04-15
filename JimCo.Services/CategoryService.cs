using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly IProductRepository _productRepository;

  public CategoryService(ICategoryRepository categoryRepository, IProductRepository productRepository)
  {
    _categoryRepository = categoryRepository;
    _productRepository = productRepository;
  }

  public async Task<int> CountAsync() => await _categoryRepository.CountAsync();

  private async Task<ApiError> ValidateModelAsync(CategoryModel model, bool checkid = false, bool update = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Name))
    {
      return new(Strings.InvalidModel);
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
    if (string.IsNullOrWhiteSpace(model.Background))
    {
      model.Background = "White";
    }
    if (string.IsNullOrWhiteSpace(model.Image))
    {
      model.Image = string.Empty;
    }
    var existing = await _categoryRepository.ReadAsync(model.Name);
    if (update)
    {
      if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
      {
        return new(string.Format(Strings.Duplicate, "Category", "name", model.Name));
      }
    }
    else if (existing is not null)
    {
      return new(string.Format(Strings.Duplicate, "Category", "name", model.Name));
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(CategoryModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    CategoryEntity entity = model!;
    try
    {
      var result = await _categoryRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(CategoryModel model)
  {
    var checkresult = await ValidateModelAsync(model, true, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    CategoryEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _categoryRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(CategoryModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    if (!model.CanDelete)
    {
      return new(string.Format(Strings.CantDelete, "category", "products"));
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "category", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _categoryRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private async Task<IEnumerable<CategoryModel>> Finish(IEnumerable<CategoryEntity> entities)
  {
    var models = entities.ToModels<CategoryModel, CategoryEntity>();
    foreach (var model in models)
    {
      model.CanDelete = !(await _productRepository.CategoryHasProductsAsync(IdEncoder.DecodeId(model.Id)));
    }
    return models;
  }

  public async Task<IEnumerable<CategoryModel>> GetAsync()
  {
    var entities = await _categoryRepository.GetAsync();
    return await Finish(entities);
  }

  public async Task<IEnumerable<CategoryModel>> SearchAsync(string searchText)
  {
    var entities = await _categoryRepository.SearchAsync(searchText);
    return await Finish(entities);
  }

  private async Task<CategoryModel?> Finish(CategoryEntity? entity)
  {
    CategoryModel ret = entity!;
    if (ret is not null)
    {
      ret.CanDelete = !await _productRepository.CategoryHasProductsAsync(entity!.Id);
    }
    return ret;
  }

  public async Task<CategoryModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    var entity = await _categoryRepository.ReadAsync(decodedid);
    return await Finish(entity);
  }

  public async Task<CategoryModel?> ReadByNameAsync(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      return null;
    }
    var entity = await _categoryRepository.ReadAsync(name);
    return await Finish(entity);
  }
}
