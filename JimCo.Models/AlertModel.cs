
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
  public string Identifier { get; set; }
  public string Title { get; set; }
  public string Text { get; set; }
  public DateTime CreateDate { get; set; }
  public string Creator { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool RequiresAcknowledgement { get; set; }
  public bool Acknowledged { get; set; }
  public DateTime AcknowledgedOn { get; set; }

  public void SetRoles(params string[] roles) => 
    Roles = roles is null || !roles.Any() ? string.Empty : JsonConvert.SerializeObject(roles.Distinct());

  public string[] GetRoles() => 
    string.IsNullOrWhiteSpace(Roles) ? Array.Empty<string>() : JsonConvert.DeserializeObject<string[]>(Roles) ?? Array.Empty<string>();

  public AlertModel() : base(true)
  {
    Id = string.Empty;
    Level = AlertLevel.Information;
    Roles = string.Empty;
    Identifier = string.Empty;
    Title = string.Empty;
    Text = string.Empty;
    CreateDate = DateTime.UtcNow;
    Creator = string.Empty;
    StartDate = default;
    EndDate = DateTime.MaxValue;
    RequiresAcknowledgement = false;
    Acknowledged = false;
    AcknowledgedOn = default;
  }

  public static AlertModel? FromEntity(AlertEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Level = entity.Level,
    Roles = entity.Roles ?? string.Empty,
    Identifier = entity.Identifier ?? string.Empty,
    Title = entity.Title ?? string.Empty,
    Text = entity.Text ?? string.Empty,
    CreateDate = entity.CreateDate,
    Creator = entity.Creator ?? string.Empty,
    StartDate = entity.StartDate,
    EndDate = entity.EndDate,
    CanDelete = true,
    RequiresAcknowledgement = entity.RequiresAcknowledgement,
    Acknowledged = entity.Acknowledged,
    AcknowledgedOn = entity.AcknowledgedOn
  };

  public static AlertEntity? FromModel(AlertModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Level = model.Level,
    Roles = model.Roles ?? string.Empty,
    Identifier = model.Identifier ?? string.Empty,
    Title = model.Title ?? string.Empty,
    Text = model.Text ?? string.Empty,
    CreateDate = model.CreateDate,
    Creator = model.Creator ?? string.Empty,
    StartDate = model.StartDate,
    EndDate = model.EndDate,
    RequiresAcknowledgement= model.RequiresAcknowledgement,
    Acknowledged= model.Acknowledged,
    AcknowledgedOn= model.AcknowledgedOn
  };

  public AlertModel Clone() => new()
  {
    Id = Id,
    Level = Level,
    Roles = Roles ?? string.Empty,
    Identifier = Identifier ?? string.Empty,
    Title = Title ?? string.Empty,
    Text = Text ?? string.Empty,
    CreateDate = CreateDate,
    Creator = Creator ?? string.Empty,
    StartDate = StartDate,
    EndDate = EndDate,
    CanDelete = CanDelete,
    RequiresAcknowledgement =RequiresAcknowledgement,
    Acknowledged = Acknowledged,
    AcknowledgedOn = AcknowledgedOn
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
