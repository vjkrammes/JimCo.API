
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class LineItemModel : ModelBase, IEquatable<LineItemModel>
{
  public string Id { get; set; }
  public string OrderId { get; set; }
  public string ProductId { get; set; }
  public int Quantity { get; set; }
  public decimal Price { get; set; }
  public int AgeRequired { get; set; }
  public DateTime StatusDate { get; set; }
  public OrderStatus Status { get; set; }

  public ProductModel? Product { get; set; }

  public LineItemModel() : base(true)
  {
    Id = string.Empty;
    OrderId = string.Empty;
    ProductId = string.Empty;
    Quantity = 0;
    Price = 0M;
    AgeRequired = 0;
    StatusDate = default;
    Status = OrderStatus.Unspecified;
    Product = null;
  }

  public static LineItemModel? FromEntity(LineItemEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    OrderId = IdEncoder.EncodeId(entity.OrderId),
    ProductId = IdEncoder.EncodeId(entity.ProductId),
    Quantity = entity.Quantity,
    Price = entity.Price,
    AgeRequired = entity.AgeRequired,
    StatusDate = entity.StatusDate,
    Status = entity.Status,
    Product = entity.Product!,
    CanDelete = true
  };

  public static LineItemEntity? FromModel(LineItemModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    OrderId = IdEncoder.DecodeId(model.OrderId),
    ProductId = IdEncoder.DecodeId(model.ProductId),
    Quantity = model.Quantity,
    Price = model.Price,
    AgeRequired = model.AgeRequired,
    StatusDate = model.StatusDate,
    Status = model.Status,
    Product = model.Product!
  };

  public LineItemModel Clone() => new()
  {
    Id = Id ?? string.Empty,
    OrderId = OrderId ?? string.Empty,
    ProductId = ProductId ?? string.Empty,
    Quantity = Quantity,
    Price = Price,
    AgeRequired = AgeRequired,
    StatusDate = StatusDate,
    Status = Status,
    Product = Product?.Clone(),
    CanDelete = CanDelete
  };

  public override string ToString() => $"{Quantity} x {Product?.Name ?? "Unknown"}";

  public override bool Equals(object? obj) => obj is LineItemModel model && model.Id == Id;

  public bool Equals(LineItemModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(LineItemModel left, LineItemModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(LineItemModel left, LineItemModel right) => !(left == right);

  public static implicit operator LineItemModel?(LineItemEntity entity) => FromEntity(entity);

  public static implicit operator LineItemEntity?(LineItemModel model) => FromModel(model);
}
