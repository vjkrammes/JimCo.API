using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Alerts")]
[BuildOrder(3)]
public class AlertEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required]
  [Indexed]
  public AlertLevel Level { get; set; }
  [Required]
  public string Roles { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  [Indexed]
  public string Identifier { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Title { get; set; }
  [Required]
  public string Text { get; set; }
  [Required]
  [Indexed]
  public DateTime CreateDate { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string Creator { get; set; }
  [Required]
  [Indexed]
  public DateTime StartDate { get; set; }
  [Required]
  [Indexed]
  public DateTime EndDate { get; set; }
  [Required]
  public bool RequiresAcknowledgement { get; set; }
  [Required]
  public bool Acknowledged { get; set; }
  [Required]
  public DateTime AcknowledgedOn { get; set; }

  public override string ToString() => Title;

  public AlertEntity()
  {
    Id = 0;
    Level = AlertLevel.Notice;
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

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Alerts (" +
    "Id integer constraint PKAlert primary key identity (1,1) not null, " +
    "Level integer not null, " +
    "Roles nvarchar(max) not null, " +
    "Identifier nvarchar(256) not null, " +
    "Title nvarchar(50) not null, " +
    "[Text] nvarchar(max) not null, " +
    "CreateDate datetime2 not null, " +
    "Creator nvarchar(256) not null, " + 
    "StartDate date not null, " +
    "EndDate date not null, " +
    "RequiresAcknowledgement bit default((0)) not null, " +
    "Acknowledged bit default ((0)) not null, " +
    "AcknowledgedOn datetime2 not null " +
    ");";
}
