
using JimCo.Common.Interfaces;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Microsoft.Extensions.Configuration;

namespace JimCo.DataAccess;
public class ProductSeeder : IProductSeeder
{
  private readonly IProductRepository _productRepository;
  private readonly ICategoryRepository _categoryRepository;
  private readonly IVendorRepository _vendorRepository;
  private readonly ILoremIpsumGenerator _loremIpsumGenerator;
  private readonly ISkuGenerator _skuGenerator;

  public ProductSeeder(IProductRepository productRepository, ICategoryRepository categoryRepository, IVendorRepository vendorRepository,
    ILoremIpsumGenerator loremIpsumGenerator, ISkuGenerator skuGenerator)
  {
    _productRepository = productRepository;
    _categoryRepository = categoryRepository;
    _vendorRepository = vendorRepository;
    _loremIpsumGenerator = loremIpsumGenerator;
    _skuGenerator = skuGenerator;
  }

  public async Task SeedAsync(IConfiguration configuration, string sectionName)
  {
    if (configuration is null || string.IsNullOrWhiteSpace(sectionName))
    {
      return;
    }
    var models = configuration.GetSection(sectionName).Get<ProductSeedModel[]>();
    if (models is null || !models.Any())
    {
      return;
    }
    foreach (var model in models)
    {
      if (string.IsNullOrWhiteSpace(model.Category) || string.IsNullOrWhiteSpace(model.Vendor) || string.IsNullOrWhiteSpace(model.Name))
      {
        continue;
      }
      if (model.Price <= 0M || model.Cost <= 0M || model.Quantity < 0 || model.ReorderLevel < 0 || model.ReorderAmount < 0 || model.AgeRequired < 0)
      {
        continue;
      }
      if (string.IsNullOrWhiteSpace(model.Description))
      {
        model.Description = _loremIpsumGenerator.Generate(10, 20, 2, 3, 1);
      }
      if (string.IsNullOrWhiteSpace(model.Sku))
      {
        model.Sku = _skuGenerator.GenerateSku();
      }
      var category = await _categoryRepository.ReadAsync(model.Category);
      if (category is null)
      {
        Console.WriteLine($"Unable to seed product '{model.Name}': Category '{model.Category}' not found");
        continue;
      }
      var vendor = await _vendorRepository.ReadAsync(model.Vendor);
      if (vendor is null)
      {
        Console.WriteLine($"Unable to seed product '{model.Name}': Vendor '{model.Vendor}' not found");
        continue;
      }
      var existing = await _productRepository.ReadForNameAsync(vendor.Id, model.Name);
      if (existing is not null)
      {
        continue;
      }
      var product = new ProductEntity()
      {
        Id = 0,
        CategoryId = category.Id,
        VendorId = vendor.Id,
        Name = model.Name,
        Description = model.Description,
        Sku = model.Sku,
        Price = model.Price,
        AgeRequired = model.AgeRequired,
        Quantity = model.Quantity,
        ReorderLevel = model.ReorderLevel,
        ReorderAmount = model.ReorderAmount,
        Cost = model.Cost,
        Discontinued = model.Discontinued,
        Category = category,
        Vendor = vendor,
        Promotions = new()
      };
      var result = await _productRepository.InsertAsync(product);
      if (!result.Successful)
      {
        Console.WriteLine($"Unable to seed product '{model.Name}': {result.ErrorMessage}");
      }
    }
  }
}
