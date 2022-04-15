
using JimCo.Common;
using JimCo.DataAccess.Entities;
using JimCo.DataAccess.Interfaces;
using JimCo.DataAccess.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace JimCo.DataAccess;
public class CategorySeeder : ICategorySeeder
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly AppSettings _settings;

  public CategorySeeder(ICategoryRepository categoryRepository, IOptions<AppSettings> settings)
  {
    _categoryRepository = categoryRepository;
    _settings = settings.Value;
  }

  public async Task SeedAsync(IConfiguration configuration, string sectionName)
  {
    if (configuration is null || string.IsNullOrWhiteSpace(sectionName))
    {
      return;
    }
    var models = configuration.GetSection(sectionName).Get<CategorySeedModel[]>();
    if (models is null || !models.Any())
    {
      return;
    }
    foreach (var model in models)
    {
      if (string.IsNullOrWhiteSpace(model.Name))
      {
        continue;
      }
      if (await _categoryRepository.ReadAsync(model.Name) is not null)
      {
        continue;
      }
      if (string.IsNullOrWhiteSpace(model.Background))
      {
        model.Background = _settings.DefaultBackground;
      }
      if (model.AgeRequired < 0)
      {
        model.AgeRequired = 0;
      }
      var category = new CategoryEntity()
      {
        Id = 0,
        Name = model.Name,
        Background = model.Background,
        IsAgeRestricted = model.AgeRequired != 0,
        AgeRequired = model.AgeRequired,
        Image = model.Image ?? string.Empty
      };
      var result = await _categoryRepository.InsertAsync(category);
      if (!result.Successful)
      {
        Console.WriteLine($"Failed to insert category '{model.Name}': {result.ErrorMessage}");
      }
    }
  }
}
