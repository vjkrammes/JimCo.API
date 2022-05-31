using System.ComponentModel.DataAnnotations;

using Dapper.Contrib.Extensions;

using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess.Entities;

[Table("SystemSettings")]
[BuildOrder(1)]
public class SystemSettingsEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required]
  public Guid SystemId { get; set; }
  [Required]
  public DateTime InceptionDate { get; set; }
  [Required]
  public string Banner { get; set; }

  public SystemSettingsEntity()
  {
    Id = 0;
    SystemId = Guid.NewGuid();
    InceptionDate = DateTime.Now;
    Banner = string.Empty;
  }

  [Write(false)]
  [JsonIgnore]
  public static SystemSettingsEntity Default => new();

  [Write(false)]
  [JsonIgnore]
  public static string Sql => "create table SystemSettings (" +
    "Id integer constraint PkSystemSettings primary key identity (1,1) not null, " +
    "SystemId UniqueIdentifier not null, " +
    "InceptionDate datetime2 not null, " +
    "Banner nvarchar(max) not null " +
    ");";
}
