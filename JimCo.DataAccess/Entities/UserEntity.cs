using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Users")]
[BuildOrder(7)]
public class UserEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, MaxLength(Constants.UriLength), Indexed]
  public string Identifier { get; set; }
  [Required, MaxLength(Constants.UriLength), Indexed]
  public string Email { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string FirstName { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string LastName { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string DisplayName { get; set; }
  [Required]
  public DateTime DateJoined { get; set; }
  [Required]
  public string JobTitles { get; set; }

  public override string ToString() => Email;

  public UserEntity()
  {
    Id = 0;
    Identifier = string.Empty;
    Email = string.Empty;
    FirstName = string.Empty;
    LastName = string.Empty;
    DisplayName = string.Empty;
    DateJoined = default;
    JobTitles = string.Empty;
  }

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Users (" +
    "Id integer constraint PKUser primary key identity (1,1) not null, " +
    "Identifier nvarchar(256) not null, " +
    "Email nvarchar(256) not null, " +
    "FirstName nvarchar(50) not null, " +
    "LastName nvarchar(50) not null, " +
    "DisplayName nvarchar(256) not null, " +
    "DateJoined datetime2 not null, " +
    "JobTitles nvarchar(max) not null, " +
    "constraint UniqueIdentifier unique nonclustered (Identifier asc), " +
    "constraint UniqueEmail unique nonclustered (Email asc) " +
    ");";
}
