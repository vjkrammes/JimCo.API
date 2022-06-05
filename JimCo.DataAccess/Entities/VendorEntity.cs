using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Vendors")]
[BuildOrder(4)]
public class VendorEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  [Indexed]
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
  [Required, MaxLength(Constants.NameLength)]
  public string Contact { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string Email { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  [Indexed]
  public string Phone { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Fax { get; set; }

  public override string ToString() => Name;

  public VendorEntity()
  {
    Id = 0;
    Name = string.Empty;
    Address1 = string.Empty;
    Address2 = string.Empty;
    City = string.Empty;
    State = string.Empty;
    PostalCode = string.Empty;
    Contact = string.Empty;
    Email = string.Empty;
    Phone = string.Empty;
    Fax = string.Empty;
  }

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Vendors (" +
    "Id integer constraint PKVendor primary key identity (1,1) not null, " +
    "Name nvarchar(50) not null, " +
    "Address1 nvarchar(50) not null, " +
    "Address2 nvarchar(50) not null, " +
    "City nvarchar(50) not null, " +
    "State nvarchar(50) not null, " +
    "PostalCode nvarchar(50) not null, " +
    "Contact nvarchar(50) not null, " +
    "Email nvarchar(50) not null, " +
    "Phone nvarchar(50) not null, " +
    "Fax nvarchar(50) not null, " +
    "constraint UniqueEmail unique nonclustered (Email asc) " +
    ");";
}
