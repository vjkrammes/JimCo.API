
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

using Newtonsoft.Json;

namespace JimCo.Models;

public class AlertModel : ModelBase, IEquatable<AlertModel>
{
  public string Id { get; set; }
  public AlertLevel Level { get; set; }
  public string Roles { get; set; }
  public string Title { get; set; }
  public string Text { get; set; }
  public DateTime CreateDate { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }

  public void SetRoles(params string[] roles) => 
    Roles = roles is null || !roles.Any() ? string.Empty : JsonConvert.SerializeObject(roles.Distinct());

  public string[] GetRoles() => 
    string.IsNullOrWhiteSpace(Roles) ? Array.Empty<string>() : JsonConvert.DeserializeObject<string[]>(Roles) ?? Array.Empty<string>();

  public AlertModel() : base(true)
  {
    Id = string.Empty;
    Level = AlertLevel.Information;
    Roles = string.Empty;
    Title = string.Empty;
    Text = string.Empty;
    CreateDate = DateTime.UtcNow;
    StartDate = default;
    EndDate = DateTime.MaxValue;
  }

  public static AlertModel? FromEntity(AlertEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Level = entity.Level,
    Roles = entity.Roles ?? string.Empty,
    Title = entity.Title ?? string.Empty,
    Text = entity.Text ?? string.Empty,
    CreateDate = entity.CreateDate,
    StartDate = entity.StartDate,
    EndDate = entity.EndDate,
    CanDelete = true
  };

  public static AlertEntity? FromModel(AlertModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Level = model.Level,
    Roles = model.Roles ?? string.Empty,
    Title = model.Title ?? string.Empty,
    Text = model.Text ?? string.Empty,
    CreateDate = model.CreateDate,
    StartDate = model.StartDate,
    EndDate = model.EndDate
  };

  public AlertModel Clone() => new()
  {
    Id = Id,
    Level = Level,
    Roles = Roles ?? string.Empty,
    Title = Title ?? string.Empty,
    Text = Text ?? string.Empty,
    CreateDate = CreateDate,
    StartDate = StartDate,
    EndDate = EndDate,
    CanDelete = CanDelete
  };

  public override string ToString() => Title;

  public override bool Equals(object? obj) => obj is AlertModel model && model.Id == Id;

  public bool Equals(AlertModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(AlertModel left, AlertModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(AlertModel left, AlertModel right) => !(left == right);

  public static implicit operator AlertModel?(AlertEntity entity) => FromEntity(entity!);

  public static implicit operator AlertEntity?(AlertModel model) => FromModel(model!);
}
