using System.ComponentModel.DataAnnotations;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess.Entities;

[Table("Groups")]
[BuildOrder(10)]
public class GroupEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  [Indexed]
  public string Name { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  [Indexed]
  public string Identifier { get; set; }

  public GroupEntity()
  {
    Id = 0;
    Name = string.Empty;
    Identifier = string.Empty;
  }

  public override string ToString() => Name;

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Groups(" +
    "Id integer constraint PKGroup primary key identity (1,1) not null, " +
    "Name nvarchar(50) not null, " +
    "Identifier nvarchar(256) not null, " +
    "Constraint UniqueGroupIdentity unique nonclustered (Name asc, Identifier asc) " +
    ");";
}
