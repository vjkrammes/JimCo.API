using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class ProductModel : ModelBase, IEquatable<ProductModel>, IComparable<ProductModel>
{
  public string Id { get; set; }
  public string CategoryId { get; set; }
  public string VendorId { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public string Sku { get; set; }
  public decimal Price { get; set; }
  public int AgeRequired { get; set; }
  public int Quantity { get; set; }
  public int ReorderLevel { get; set; }
  public int ReorderAmount { get; set; }
  public decimal Cost { get; set; }
  public bool Discontinued { get; set; }

  public CategoryModel? Category { get; set; }

  public VendorModel? Vendor { get; set; }

  public List<PromotionModel> Promotions { get; set; }
  public PromotionModel? CurrentPromotion { get; init; }

  public ProductModel()
  {
    Id = string.Empty;
    CategoryId = string.Empty;
    VendorId = string.Empty;
    Name = string.Empty;
    Description = string.Empty;
    Sku = string.Empty;
    Price = 0M;
    AgeRequired = 0;
    Quantity = 0;
    ReorderLevel = 0;
    ReorderAmount = 0;
    Cost = 0M;
    Discontinued = false;
    Category = null;
    Vendor = null;
    Promotions = new();
    CurrentPromotion = null;
  }

  public static ProductModel? FromEntity(ProductEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    CategoryId = IdEncoder.EncodeId(entity.CategoryId),
    VendorId = IdEncoder.EncodeId(entity.VendorId),
    Name = entity.Name ?? string.Empty,
    Description = entity.Description ?? string.Empty,
    Sku = entity.Sku ?? string.Empty,
    Price = entity.Price,
    AgeRequired = entity.AgeRequired,
    Quantity = entity.Quantity,
    ReorderLevel = entity.ReorderLevel,
    ReorderAmount = entity.ReorderAmount,
    Cost = entity.Cost,
    Discontinued = entity.Discontinued,
    Category = entity.Category!,
    Vendor = entity.Vendor!,
    Promotions = entity.Promotions?.ToModels<PromotionModel, PromotionEntity>().ToList() ?? new List<PromotionModel>(),
    CurrentPromotion = entity.Promotions?.FirstOrDefault(x => x.StartDate <= DateTime.UtcNow && x.StopDate >= DateTime.UtcNow)!
  };

  public static ProductEntity? FromModel(ProductModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    CategoryId = IdEncoder.DecodeId(model.CategoryId),
    VendorId = IdEncoder.DecodeId(model.VendorId),
    Name = model.Name ?? string.Empty,
    Description = model.Description ?? string.Empty,
    Sku = model.Sku ?? string.Empty,
    Price = model.Price,
    AgeRequired = model.AgeRequired,
    Quantity = model.Quantity,
    ReorderLevel = model.ReorderLevel,
    ReorderAmount = model.ReorderAmount,
    Cost = model.Cost,
    Discontinued = model.Discontinued,
    Category = model.Category!,
    Vendor = model.Vendor!,
    Promotions = new(model.Promotions?.Select(x => PromotionModel.FromModel(x))!)
  };

  public ProductModel Clone() => new()
  {
    Id = Id,
    CategoryId = CategoryId,
    VendorId = VendorId,
    Name = Name ?? string.Empty,
    Description = Description ?? string.Empty,
    Sku = Sku ?? string.Empty,
    Price = Price,
    AgeRequired = AgeRequired,
    Quantity = Quantity,
    ReorderLevel = ReorderLevel,
    ReorderAmount = ReorderAmount,
    Cost = Cost,
    Discontinued = Discontinued,
    Category = Category?.Clone(),
    Vendor = Vendor?.Clone(),
    Promotions = new(Promotions),
    CurrentPromotion = CurrentPromotion?.Clone()
  };

  public override string ToString() => Name;

  public override bool Equals(object? obj) => obj is ProductModel model && model.Id == Id;

  public bool Equals(ProductModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(ProductModel left, ProductModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(ProductModel left, ProductModel right) => !(left == right);

  public int CompareTo(ProductModel? other) => Name.CompareTo(other?.Name);

  public static bool operator >(ProductModel left, ProductModel right) => left.CompareTo(right) > 0;

  public static bool operator <(ProductModel left, ProductModel right) => left.CompareTo(right) < 0;

  public static bool operator >=(ProductModel left, ProductModel right) => left.CompareTo(right) >= 0;

  public static bool operator <=(ProductModel left, ProductModel right) => left.CompareTo(right) <= 0;

  public static implicit operator ProductModel?(ProductEntity entity) => FromEntity(entity);

  public static implicit operator ProductEntity?(ProductModel model) => FromModel(model);
}
