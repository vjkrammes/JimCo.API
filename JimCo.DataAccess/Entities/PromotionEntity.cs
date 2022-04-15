using System.ComponentModel.DataAnnotations;

using Dapper.Contrib.Extensions;

using JimCo.Common;
using JimCo.Common.Attributes;
using JimCo.Common.Interfaces;

namespace JimCo.DataAccess.Entities;

[Table("Promotions")]
[BuildOrder(6)]
public class PromotionEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required]
  [Indexed]
  public int ProductId { get; set; }
  [Required]
  public DateTime CreatedOn { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string CreatedBy { get; set; }
  [Required]
  [Indexed]
  public DateTime StartDate { get; set; }
  [Required]
  [Indexed]
  public DateTime StopDate { get; set; }
  [Required]
  public DateTime CanceledOn { get; set; }
  [Required, MaxLength(Constants.UriLength)]
  public string CanceledBy { get; set; }
  [Required]
  public decimal Price { get; set; }
  [Required]
  public string Description { get; set; }
  [Required]
  public bool LimitedQuantity { get; set; }
  [Required, Positive]
  public int MaximumQuantity { get; set; }

  public override string ToString() => $"{StartDate.ToShortDateString()} - {StopDate.ToShortDateString()}: {Price:c2}";

  public PromotionEntity()
  {
    Id = 0;
    ProductId = 0;
    CreatedOn = default;
    CreatedBy = string.Empty;
    StartDate = default;
    StopDate = default;
    CanceledOn = default;
    CanceledBy = string.Empty;
    Price = 0M;
    Description = string.Empty;
    LimitedQuantity = false;
    MaximumQuantity = 0;
  }

  public static string Sql => "create table Promotions (" +
    "Id integer constraint PKPromotion primary key identity (1,1) not null, " +
    "ProductId integer not null, " +
    "CreatedOn datetime2 not null, " +
    "CreatedBy nvarchar(256) not null, " +
    "StartDate date not null, " +
    "StopDate date not null, " +
    "CanceledOn datetime2 not null, " +
    "Canceledby nvarchar(256) not null, " +
    "Price decimal(11,2) default ((0)) not null, " +
    "Description nvarchar(max) not null, " +
    "LimitedQuantity bit default ((0)) not null, " +
    "MaximumQuantity integer default ((0)) not null, " +
    "constraint FKPromotionProduct foreign key (ProductId) references Products(Id) " +
    ");";
}
