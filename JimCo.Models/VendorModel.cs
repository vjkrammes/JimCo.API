
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class VendorModel : ModelBase, IEquatable<VendorModel>, IComparable<VendorModel>
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Address1 { get; set; }
  public string Address2 { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string PostalCode { get; set; }
  public string Contact { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public string Fax { get; set; }

  public VendorModel() : base(true)
  {
    Id = string.Empty;
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    Contact = string.Empty;
    Email = string.Empty;
    Phone = string.Empty;
    Fax = string.Empty;
  }

  public static VendorModel? FromEntity(VendorEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Name = entity.Name ?? string.Empty,
    Address1 = entity.Address1 ?? string.Empty,
    Address2 = entity.Address2 ?? string.Empty,
    City = entity.City ?? string.Empty,
    State = entity.State ?? string.Empty,
    PostalCode = entity.PostalCode ?? string.Empty,
    Contact = entity.Contact ?? string.Empty,
    Email = entity.Email ?? string.Empty,
    Phone = entity.Phone ?? string.Empty,
    Fax = entity.Fax ?? string.Empty,
    CanDelete = true
  };

  public static VendorEntity? FromModel(VendorModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Name = model.Name ?? string.Empty,
    Address1 = model.Address1 ?? string.Empty,
    Address2 = model.Address2 ?? string.Empty,
    City = model.City ?? string.Empty,
    State = model.State ?? string.Empty,
    PostalCode = model.PostalCode ?? string.Empty,
    Contact = model.Contact ?? string.Empty,
    Email = model.Email ?? string.Empty,
    Phone = model.Phone ?? string.Empty,
    Fax = model.Fax ?? string.Empty
  };

  public VendorModel Clone() => new()
  {
    Id = Id,
    Name = Name ?? string.Empty,
    Address1 = Address1 ?? string.Empty,
    Address2 = Address2 ?? string.Empty,
    City = City ?? string.Empty,
    State = State ?? string.Empty,
    PostalCode = PostalCode ?? string.Empty,
    Contact = Contact ?? string.Empty,
    Email = Email ?? string.Empty,
    Phone = Phone ?? string.Empty,
    Fax = Fax ?? string.Empty,
    CanDelete = CanDelete
  };

  public override string ToString() => Name;

  public override bool Equals(object? obj) => obj is VendorModel model && model.Id == Id;

  public bool Equals(VendorModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => base.GetHashCode();

  public static bool operator ==(VendorModel left, VendorModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(VendorModel left, VendorModel right) => !(left == right);

  public int CompareTo(VendorModel? other) => Name.CompareTo(other?.Name);

  public static bool operator >(VendorModel left, VendorModel right) => left.CompareTo(right) > 0;

  public static bool operator <(VendorModel left, VendorModel right) => left.CompareTo(right) < 0;

  public static bool operator >=(VendorModel left, VendorModel right) => left.CompareTo(right) >= 0;

  public static bool operator <=(VendorModel left, VendorModel right) => left.CompareTo(right) <= 0;

  public static implicit operator VendorModel?(VendorEntity entity) => FromEntity(entity);

  public static implicit operator VendorEntity?(VendorModel model) => FromModel(model);
}
