
using System.ComponentModel.DataAnnotations;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess.Entities;

[Table("Logs")]
[BuildOrder(11)]
public class LogEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, Indexed]
  public DateTime Timestamp { get; set; }
  [Required, NonNegative]
  public Level Level { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Ip { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string Identifier { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Source { get; set; }
  [Required]
  public string Description { get; set; }
  [Required]
  public string Data { get; set; }

  public LogEntity()
  {
    Id = 0;
    Timestamp = DateTime.UtcNow;
    Level = Level.NoLevel;
    Ip = string.Empty;
    Identifier = string.Empty;
    Source = string.Empty;
    Description = string.Empty;
    Data = string.Empty;
  }

  public override string ToString() => $"{Timestamp} ({Level.GetDescriptionFromEnumValue()}) [{Source}] {Description.Beginning(10)}";

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Logs (" +
    "Id integer constraint PkLog primary key identity (1,1) not null, " +
    "Timestamp datetime2 not null, " +
    "Level integer default ((0)) not null, " +
    "Ip nvarchar(50) not null, " +
    "Identifier nvarchar(256) not null, " +
    "Source nvarchar(50) not null, " +
    "Description nvarchar(max) not null, " +
    "Data nvarchar(max) not null " +
    ");";
}
