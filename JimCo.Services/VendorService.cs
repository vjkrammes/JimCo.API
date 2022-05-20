
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.Models;
using JimCo.Services.Interfaces;

namespace JimCo.Services;
public class VendorService : IVendorService
{
  private readonly IVendorRepository _vendorRepository;
  private readonly IProductRepository _productRepository;
  private readonly IUserRepository _userRepository;

  public VendorService(IVendorRepository vendorRepository, IProductRepository productRepository, IUserRepository userRepository)
  {
    _vendorRepository = vendorRepository;
    _productRepository = productRepository;
    _userRepository = userRepository;
  }

  public async Task<int> CountAsync() => await _vendorRepository.CountAsync();

  private async Task<ApiError> ValidateModelAsync(VendorModel model, bool checkid = false, bool update = false)
  {
    if (model is null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Contact) || string.IsNullOrWhiteSpace(model.Email))
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
    var existing = (await _vendorRepository.ReadAsync(model.Name))!;
    if (update)
    {
      if (existing is not null && existing.Id != IdEncoder.DecodeId(model.Id))
      {
        return new(string.Format(Strings.Duplicate, "vendor", "name", model.Name));
      }
    }
    else if (existing is not null)
    {
      return new(string.Format(Strings.Duplicate, "vendor", "name", model.Name));
    }
    return ApiError.Success;
  }

  public async Task<ApiError> InsertAsync(VendorModel model)
  {
    var checkresult = await ValidateModelAsync(model);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    VendorEntity entity = model!;
    try
    {
      var result = await _vendorRepository.InsertAsync(entity);
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

  public async Task<ApiError> UpdateAsync(VendorModel model)
  {
    var checkresult = await ValidateModelAsync(model, true, true);
    if (!checkresult.Successful)
    {
      return checkresult;
    }
    VendorEntity entity = model!;
    try
    {
      return ApiError.FromDalResult(await _vendorRepository.UpdateAsync(entity));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  public async Task<ApiError> DeleteAsync(VendorModel model)
  {
    if (model is null)
    {
      return new(Strings.InvalidModel);
    }
    if (!model.CanDelete)
    {
      return new(string.Format(Strings.CantDelete, "vendor", "products"));
    }
    var decodedid = IdEncoder.DecodeId(model.Id);
    if (decodedid <= 0)
    {
      return new(string.Format(Strings.NotFound, "vendor", "id", model.Id));
    }
    try
    {
      return ApiError.FromDalResult(await _vendorRepository.DeleteAsync(decodedid));
    }
    catch (Exception ex)
    {
      return ApiError.FromException(ex);
    }
  }

  private async Task<IEnumerable<VendorModel>> Finish(IEnumerable<VendorEntity> entities)
  {
    var models = entities.ToModels<VendorModel, VendorEntity>();
    foreach (var model in models)
    {
      model.CanDelete = !await _productRepository.VendorHasProductsAsync(IdEncoder.DecodeId(model.Id));
      UserModel? user = (await _userRepository.ReadAsync(model.Email))!;
      model.UserExists = user is not null;
      model.HasVendorRole = user?.HasVendorRole ?? false;
    }
    return models;
  }

  public async Task<IEnumerable<VendorModel>> GetAsync()
  {
    var entities = await _vendorRepository.GetAsync();
    return await Finish(entities);
  }

  public async Task<IEnumerable<VendorModel>> PageVendorsAsync(int pageno, int pagesize, string columnName = "Id")
  {
    var entities = await _vendorRepository.PageVendorsAsync(pageno, pagesize, columnName);
    return await Finish(entities);
  }

  public async Task<IEnumerable<VendorModel>> SearchAsync(string searchText)
  {
    var entities = await _vendorRepository.SearchAsync(searchText);
    return await Finish(entities);
  }

  public async Task<IEnumerable<VendorModel>> SearchForContactAsync(string contact)
  {
    var entities = await _vendorRepository.SearchContactAsync(contact);
    return await Finish(entities);
  }

  public async Task<IEnumerable<VendorModel>> SearchForEmailAsync(string email)
  {
    var entities = await _vendorRepository.SearchEmailAsync(email);
    return await Finish(entities);
  }

  private async Task<VendorModel?> Finish(VendorEntity? entity)
  {
    VendorModel ret = entity!;
    if (ret is not null)
    {
      ret.CanDelete = !await _productRepository.VendorHasProductsAsync(entity!.Id);
    }
    return ret;
  }

  public async Task<VendorModel?> ReadAsync(string id)
  {
    var decodedid = IdEncoder.DecodeId(id);
    if (decodedid <= 0)
    {
      return null;
    }
    return await Finish(await _vendorRepository.ReadAsync(decodedid))!;
  }

  public async Task<VendorModel?> ReadForNameAsync(string name) => await Finish(await _vendorRepository.ReadAsync(name))!;
}
