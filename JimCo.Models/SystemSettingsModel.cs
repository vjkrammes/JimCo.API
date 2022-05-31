
using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class SystemSettingsModel : ModelBase, IEquatable<SystemSettingsModel>
{
  public string Id { get; set; }
  public Guid SystemId { get; set; }
  public DateTime InceptionDate { get; set; }
  public string Banner { get; set; }

  public SystemSettingsModel()
  {
    Id = string.Empty;
    SystemId = Guid.Empty;
    InceptionDate = default;
    Banner = string.Empty;
  }

  public static SystemSettingsModel? FromEntity(SystemSettingsEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    SystemId = entity.SystemId,
    InceptionDate = entity.InceptionDate,
    Banner = entity.Banner ?? string.Empty,
    CanDelete = false
  };

  public static SystemSettingsEntity? FromModel(SystemSettingsModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    SystemId = model.SystemId,
    InceptionDate = model.InceptionDate,
    Banner= model.Banner ?? string.Empty
  };

  public SystemSettingsModel Clone() => new()
  {
    Id = Id ?? string.Empty,
    SystemId = SystemId,
    InceptionDate = InceptionDate,
    Banner = Banner ?? string.Empty,
    CanDelete = false
  };

  public override string ToString() => SystemId.ToString();

  public override bool Equals(object? obj) => obj is SystemSettingsModel model && model.Id == Id;

  public bool Equals(SystemSettingsModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => SystemId.GetHashCode();

  public static bool operator ==(SystemSettingsModel left, SystemSettingsModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(SystemSettingsModel left, SystemSettingsModel right) => !(left == right);

  public static implicit operator SystemSettingsModel?(SystemSettingsEntity entity) => FromEntity(entity);

  public static implicit operator SystemSettingsEntity?(SystemSettingsModel model) => FromModel(model);
}
