using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Products")]
[BuildOrder(5)]
public class ProductEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required]
  public int CategoryId { get; set; }
  [Required]
  public int VendorId { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Name { get; set; }
  [Required]
  public string Description { get; set; }
  [Required, MaxLength(Constants.NameLength)]
  public string Sku { get; set; }
  [Required]
  public decimal Price { get; set; }
  [Required]
  public int AgeRequired { get; set; }
  [Required, NonNegative]
  public int Quantity { get; set; }
  [Required, NonNegative]
  public int ReorderLevel { get; set; }
  [Required, Positive]
  public int ReorderAmount { get; set; }
  [Required, Positive]
  public decimal Cost { get; set; }
  [Required]
  public bool Discontinued { get; set; }

  [Write(false)]
  public CategoryEntity? Category { get; set; }

  [Write(false)]
  public VendorEntity? Vendor { get; set; }

  [Write(false)]
  public List<PromotionEntity> Promotions { get; set; }

  public override string ToString() => Name;

  public ProductEntity()
  {
    Id = 0;
    CategoryId = 0;
    VendorId = 0;
    Name = string.Empty;
    Description = string.Empty;
    Sku = string.Empty;
    Price = 0M;
    AgeRequired = 0;
    Quantity = 0;
    ReorderLevel = 0;
    ReorderAmount = 0;
    Cost = 0M;
    Discontinued = false;
    Category = null;
    Vendor = null;
    Promotions = new();
  }

  [JsonIgnore]
  [Write(false)]
  public static string Sql => "create table Products (" +
    "Id integer constraint PKProduct primary key identity (1,1) not null, " +
    "CategoryId integer not null, " +
    "VendorId integer not null, " +
    "Name nvarchar(50) not null, " +
    "Description nvarchar(max) not null, " +
    "Sku nvarchar(50) not null, " +
    "Price decimal(11,2) default ((0)) not null, " +
    "AgeRequired integer not null, " +
    "Quantity integer not null, " +
    "ReorderLevel integer not null, " +
    "ReorderAmount integer not null, " +
    "Cost decimal(11,2) default ((0)) not null, " +
    "Discontinued bit default ((0)) not null, " +
    "constraint UniqueProduct unique nonclustered (VendorId asc, Name asc), " +
    "constraint FKProductCategory foreign key (CategoryId) references Categories(Id), " +
    "constraint FKProductVendor foreign key (VendorId) references Vendors(Id) " +
    ");";
}
