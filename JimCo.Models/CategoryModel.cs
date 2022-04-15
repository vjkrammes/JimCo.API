
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;

public class CategoryModel : ModelBase, IEquatable<CategoryModel>, IComparable<CategoryModel>
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Background { get; set; }
  public bool IsAgeRestricted { get; set; }
  public int AgeRequired { get; set; }
  public string Image { get; set; }

  public CategoryModel() : base(true)
  {
    Id = string.Empty;
    Name = string.Empty;
    Background = string.Empty;
    IsAgeRestricted = false;
    AgeRequired = 0;
    Image = string.Empty;
  }

  public static CategoryModel? FromEntity(CategoryEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Name = entity.Name ?? string.Empty,
    Background = entity.Background ?? string.Empty,
    IsAgeRestricted = entity.IsAgeRestricted,
    AgeRequired = entity.AgeRequired,
    Image = entity.Image ?? string.Empty,
    CanDelete = true
  };

  public static CategoryEntity? FromModel(CategoryModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Name = model.Name ?? string.Empty,
    Background = model.Background ?? string.Empty,
    IsAgeRestricted = model.IsAgeRestricted,
    AgeRequired = model.AgeRequired,
    Image = model.Image ?? string.Empty
  };

  public CategoryModel Clone() => new()
  {
    Id = Id,
    Name = Name ?? string.Empty,
    Background = Background ?? string.Empty,
    IsAgeRestricted = IsAgeRestricted,
    AgeRequired = AgeRequired,
    Image = Image ?? string.Empty,
    CanDelete = CanDelete
  };

  public override string ToString() => Name;

  public override bool Equals(object? obj) => obj is CategoryModel model && model.Id == Id;

  public bool Equals(CategoryModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(CategoryModel left, CategoryModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(CategoryModel left, CategoryModel right) => !(left == right);

  public int CompareTo(CategoryModel? other) => Name.CompareTo(other?.Name);

  public static bool operator >(CategoryModel left, CategoryModel right) => left.CompareTo(right) > 0;

  public static bool operator <(CategoryModel left, CategoryModel right) => left.CompareTo(right) < 0;

  public static bool operator >=(CategoryModel left, CategoryModel right) => left.CompareTo(right) >= 0;

  public static bool operator <=(CategoryModel left, CategoryModel right) => left.CompareTo(right) <= 0;

  public static implicit operator CategoryModel?(CategoryEntity entity) => FromEntity(entity);

  public static implicit operator CategoryEntity?(CategoryModel model) => FromModel(model);
}
