using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Categories")]
[BuildOrder(2)]
public class CategoryEntity : IIdEntity,  ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Name { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Background { get; set; }
  [Required]
  public bool IsAgeRestricted { get; set; }
  [Required, NonNegative]
  public int AgeRequired { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string Image { get; set; }

  public override string ToString() => Name;

  public CategoryEntity()
  {
    Id = 0;
    Name = string.Empty;
    Background = string.Empty;
    IsAgeRestricted = false;
    AgeRequired = 0;
    Image = string.Empty;
  }

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Categories (" +
    "Id integer constraint PKCategory primary key identity (1,1) not null, " +
    "Name nvarchar(50) not null, " +
    "Background nvarchar(50) not null, " +
    "IsAgeRestricted bit default ((0)) not null, " +
    "AgeRequired integer default ((0)) not null, " +
    "Image nvarchar(256) not null, " +
    "constraint UniqueName unique nonclustered (Name asc) " +
    ");";
}
