
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;

public class PromotionModel : ModelBase, IEquatable<PromotionModel>
{
  public string Id { get; set; }
  public string ProductId { get; set; }
  public DateTime CreatedOn { get; set; }
  public string CreatedBy { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime StopDate { get; set; }
  public DateTime CanceledOn { get; set; }
  public string CanceledBy { get; set; }
  public decimal Price { get; set; }
  public string Description { get; set; }
  public bool LimitedQuantity { get; set; }
  public int MaximumQuantity { get; set; }

  public PromotionModel() : base(true)
  {
    Id = string.Empty;
    ProductId = string.Empty;
    CreatedOn = default;
    CreatedBy = string.Empty;
    StartDate = default;
    StopDate = default;
    CanceledOn = default;
    CanceledBy = string.Empty;
    Price = 0M;
    Description = string.Empty;
    LimitedQuantity = false;
    MaximumQuantity = 0;
  }

  public static PromotionModel? FromEntity(PromotionEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    ProductId = IdEncoder.EncodeId(entity.ProductId),
    CreatedOn = entity.CreatedOn,
    CreatedBy = entity.CreatedBy ?? string.Empty,
    StartDate = entity.StartDate,
    StopDate = entity.StopDate,
    CanceledOn = entity.CanceledOn,
    CanceledBy = entity.CanceledBy ?? string.Empty,
    Price = entity.Price,
    Description = entity.Description ?? string.Empty,
    LimitedQuantity = entity.LimitedQuantity,
    MaximumQuantity = entity.MaximumQuantity,
    CanDelete = true
  };

  public static PromotionEntity? FromModel(PromotionModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    ProductId = IdEncoder.DecodeId(model.ProductId),
    CreatedOn = model.CreatedOn,
    CreatedBy = model.CreatedBy ?? string.Empty,
    StartDate = model.StartDate,
    StopDate = model.StopDate,
    CanceledOn = model.CanceledOn,
    CanceledBy = model.CanceledBy ?? string.Empty,
    Price = model.Price,
    Description = model.Description ?? string.Empty,
    LimitedQuantity = model.LimitedQuantity,
    MaximumQuantity = model.MaximumQuantity
  };

  public PromotionModel Clone() => new()
  {
    Id = Id,
    ProductId = ProductId,
    CreatedOn = CreatedOn,
    CreatedBy = CreatedBy ?? string.Empty,
    StartDate = StartDate,
    StopDate = StopDate,
    CanceledOn = CanceledOn,
    CanceledBy = CanceledBy ?? string.Empty,
    Price = Price,
    Description = Description ?? string.Empty,
    LimitedQuantity = LimitedQuantity,
    MaximumQuantity = MaximumQuantity,
    CanDelete = CanDelete
  };

  public override string ToString() => $"{StartDate.ToShortDateString()} - {StopDate.ToShortDateString()}: {Price:c2}";

  public override bool Equals(object? obj) => obj is PromotionModel model && model.Id == Id;

  public bool Equals(PromotionModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(PromotionModel left, PromotionModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(PromotionModel left, PromotionModel right) => !(left == right);

  public static implicit operator PromotionModel?(PromotionEntity entity) => FromEntity(entity);

  public static implicit operator PromotionEntity?(PromotionModel model) => FromModel(model);
}
