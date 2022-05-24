
using JimCo.Common;
using JimCo.Common.Enumerations;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;
public class LogModel : ModelBase, IEquatable<LogModel>, IComparable<LogModel>
{
  public string Id { get; set; }
  public DateTime Timestamp { get; set; }
  public Level Level { get; set; }
  public string Ip { get; set; }
  public string Identifier { get; set; }
  public string Source { get; set; }
  public string Description { get; set; }
  public string Data { get; set; }

  public LogModel()
  {
    Id = IdEncoder.EncodeId(0);
    Timestamp = DateTime.UtcNow;
    Level = Level.NoLevel;
    Ip = string.Empty;
    Identifier = string.Empty;
    Source = string.Empty;
    Description = string.Empty;
    Data = string.Empty;
  }

  public static LogModel? FromEntity(LogEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId(entity.Id),
    Timestamp = entity.Timestamp,
    Level = entity.Level,
    Ip = entity.Ip ?? string.Empty,
    Identifier = entity.Identifier ?? string.Empty,
    Source = entity.Source ?? string.Empty,
    Description = entity.Description ?? string.Empty,
    Data = entity.Data ?? string.Empty
  };

  public static LogEntity? FromModel(LogModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Timestamp = model.Timestamp,
    Level = model.Level,
    Ip = model.Ip ?? string.Empty,
    Identifier = model.Identifier ?? string.Empty,
    Source = model.Source ?? string.Empty,
    Description = model.Description ?? string.Empty,
    Data = model.Data ?? string.Empty
  };

  public LogModel Clone() => new()
  {
    Id = Id ?? IdEncoder.EncodeId(0),
    Timestamp = Timestamp,
    Level = Level,
    Ip = Ip ?? string.Empty,
    Identifier = Identifier ?? string.Empty,
    Source = Source ?? string.Empty,
    Description = Description ?? string.Empty,
    Data = Data ?? string.Empty
  };

  public override string ToString() => $"{Timestamp} ({Level.GetDescriptionFromEnumValue()}) [{Source}] {Description.Beginning(10)}";

  public override bool Equals(object? obj) => obj is LogModel model && model.Id == Id;

  public bool Equals(LogModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(LogModel left, LogModel right) => (left, right) switch
  {
    (null, null) => true,
    (_, null) or (null, _) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(LogModel left, LogModel right) => !(left == right);

  public int CompareTo(LogModel? other) => Timestamp.CompareTo(other?.Timestamp);

  public static bool operator >(LogModel left, LogModel right) => left.CompareTo(right) > 0;

  public static bool operator <(LogModel left, LogModel right) => left.CompareTo(right) < 0;

  public static bool operator >=(LogModel left, LogModel right) => left.CompareTo(right) >= 0;

  public static bool operator <=(LogModel left, LogModel right) => left.CompareTo(right) <= 0;

  public static implicit operator LogModel?(LogEntity entity) => FromEntity(entity);

  public static implicit operator LogEntity?(LogModel model) => FromModel(model);
}
