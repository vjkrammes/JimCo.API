using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Orders")]
[BuildOrder(8)]
public class OrderEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  [Indexed]
  public string Email { get; set; }
  [Required]
  [Indexed]
  public int Pin { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Name { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Address1 { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Address2 { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string City { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string State { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string PostalCode { get; set; }
  [Required]
  [Indexed]
  public DateTime CreateDate { get; set; }
  [Required]
  public DateTime StatusDate { get; set; }
  [Required]
  [Indexed]
  public OrderStatus Status { get; set; }
  [Required, NonNegative]
  public int AgeRequired { get; set; }

  [Write(false)]
  public List<LineItemEntity> LineItems { get; set; }

  public OrderEntity()
  {
    Id = 0;
    Email = string.Empty;
    Pin = 0;
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    CreateDate = default;
    StatusDate = default;
    Status = OrderStatus.Unspecified;
    AgeRequired = 0;
    LineItems = new();
  }

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Orders (" +
    "Id integer constraint PKOrder primary key identity (1,1) not null, " +
    "Email nvarchar(256) not null, " +
    "Pin integer not null, " +
    "Name nvarchar(50) not null, " +
    "Address1 nvarchar(50) not null, " +
    "Address2 nvarchar(50) not null, " +
    "City nvarchar(50) not null, " +
    "State nvarchar(50) not null, " +
    "PostalCode nvarchar(50) not null, " +
    "CreateDate datetime2 not null, " +
    "StatusDate datetime2 not null, " +
    "Status integer default ((0)) not null, " +
    "AgeRequired integer default ((0)) not null " +
    ");";
}
