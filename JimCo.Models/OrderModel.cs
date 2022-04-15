using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class OrderModel : ModelBase, IEquatable<OrderModel>
{
  public string Id { get; set; }
  public string Email { get; set; }
  public string Pin { get; set; }
  public string Name { get; set; }
  public string Address1 { get; set; }
  public string Address2 { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string PostalCode { get; set; }
  public DateTime CreateDate { get; set; }
  public DateTime StatusDate { get; set; }
  public OrderStatus Status { get; set; }
  public int AgeRequired { get; set; }
  public List<LineItemModel> LineItems { get; set; }

  public OrderModel()
  {
    Id = string.Empty;
    Email = string.Empty;
    Pin = string.Empty;
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    CreateDate = default;
    StatusDate = default;
    Status = OrderStatus.Unspecified;
    AgeRequired = 0;
    LineItems = new();
  }

  public static OrderModel? FromEntity(OrderEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Email = entity.Email ?? string.Empty,
    Pin = IdEncoder.EncodeId(entity.Pin),
    Name = entity.Name ?? string.Empty,
    Address1 = entity.Address1 ?? string.Empty,
    Address2 = entity.Address2 ?? string.Empty,
    City = entity.City ?? string.Empty,
    State = entity.State ?? string.Empty,
    PostalCode = entity.PostalCode ?? string.Empty,
    CreateDate = entity.CreateDate,
    StatusDate = entity.StatusDate,
    Status = entity.Status,
    AgeRequired = entity.AgeRequired,
    LineItems = entity.LineItems.ToModels<LineItemModel, LineItemEntity>().ToList() ?? new List<LineItemModel>()
  };

  public static OrderEntity? FromModel(OrderModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Email = model.Email ?? string.Empty,
    Pin = IdEncoder.DecodeId(model.Pin),
    Name = model.Name ?? string.Empty,
    Address1 = model.Address1 ?? string.Empty,
    Address2 = model.Address2 ?? string.Empty,
    City = model.City ?? string.Empty,
    State = model.State ?? string.Empty,
    PostalCode = model.PostalCode ?? string.Empty,
    CreateDate = model.CreateDate,
    StatusDate = model.StatusDate,
    Status = model.Status,
    AgeRequired = model.AgeRequired,
    LineItems = new List<LineItemEntity>(model.LineItems?.Select(x => LineItemModel.FromModel(x))!)
  };

  public OrderModel Clone() => new()
  {
    Id = Id,
    Email = Email ?? string.Empty,
    Pin = Pin,
    Name = Name ?? string.Empty,
    Address1 = Address1 ?? string.Empty,
    Address2 = Address2 ?? string.Empty,
    City = City ?? string.Empty,
    State = State ?? string.Empty,
    PostalCode = PostalCode ?? string.Empty,
    CreateDate = CreateDate,
    StatusDate = StatusDate,
    Status = Status,
    AgeRequired = AgeRequired,
    LineItems = new(LineItems),
    CanDelete = CanDelete
  };

  public override string ToString() => $"{CreateDate.ToShortDateString()} {LineItems?.Count ?? 0} Items";

  public override bool Equals(object? obj) => obj is OrderModel model && model.Id == Id;

  public bool Equals(OrderModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(OrderModel left, OrderModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(OrderModel left, OrderModel right) => !(left == right);

  public static implicit operator OrderModel?(OrderEntity entity) => FromEntity(entity);

  public static implicit operator OrderEntity?(OrderModel model) => FromModel(model);
}
