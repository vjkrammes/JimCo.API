
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class GroupModel : ModelBase, IEquatable<GroupModel>, IComparable<GroupModel>
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string UserId { get; set; }

  public GroupModel()
  {
    Id = string.Empty;
    Name = string.Empty;
    UserId = string.Empty;
  }

  public static GroupModel? FromEntity(GroupEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Name = entity.Name ?? string.Empty,
    UserId = IdEncoder.EncodeId(entity.UserId),
    CanDelete = true
  };

  public static GroupEntity? FromModel(GroupModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Name = model.Name ?? string.Empty,
    UserId = IdEncoder.DecodeId(model.UserId),
  };

  public GroupModel Clone() => new()
  {
    Id = Id,
    Name = Name ?? string.Empty,
    UserId = UserId ?? string.Empty,
    CanDelete = CanDelete
  };

  public override string ToString() => Name;

  public override bool Equals(object? obj) => obj is GroupModel model && model.Id == Id;

  public bool Equals(GroupModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(GroupModel left, GroupModel right) => (left, right) switch
  {
    (null, null) => true,
    (_, null) or (null, _) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(GroupModel left, GroupModel right) => !(left == right);

  public int CompareTo(GroupModel? other) => Name.CompareTo(other?.Name);

  public static bool operator >(GroupModel left, GroupModel right) => left.CompareTo(right) > 0;

  public static bool operator <(GroupModel left, GroupModel right) => left.CompareTo(right) < 0;

  public static bool operator >=(GroupModel left, GroupModel right) => left.CompareTo(right) >= 0;

  public static bool operator <=(GroupModel left, GroupModel right) => left.CompareTo(right) <= 0;

  public static implicit operator GroupModel?(GroupEntity entity) => FromEntity(entity);

  public static implicit operator GroupEntity?(GroupModel model) => FromModel(model);
}
