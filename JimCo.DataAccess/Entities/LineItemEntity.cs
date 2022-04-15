using System.ComponentModel.DataAnnotations;

using Dapper.Contrib.Extensions;

using JimCo.Common.Attributes;
using JimCo.Common.Enumerations;
using JimCo.Common.Interfaces;

using Newtonsoft.Json;

namespace JimCo.DataAccess.Entities;

[Table("LineItems")]
[BuildOrder(9)]
public class LineItemEntity : IIdEntity, ISqlEntity
{
  [Required]
  public int Id { get; set; }
  [Required, Indexed]
  public int OrderId { get; set; }
  [Required, Indexed]
  public int ProductId { get; set; }
  [Required, Positive]
  public int Quantity { get; set; }
  [Required, NonNegative]
  public decimal Price { get; set; }
  [Required, NonNegative]
  public int AgeRequired { get; set; }
  [Required, Indexed]
  public DateTime StatusDate { get; set; }
  [Required, Indexed]
  public OrderStatus Status { get; set; }

  [Write(false)]
  public ProductEntity? Product { get; set; }

  public LineItemEntity()
  {
    Id = 0;
    OrderId = 0;
    ProductId = 0;
    Quantity = 0;
    Price = 0M;
    AgeRequired = 0;
    StatusDate = default;
    Status = OrderStatus.Unspecified;
    Product = null;
  }

  [Write(false)]
  [JsonIgnore]
  public static string Sql => "create table LineItems (" +
    "Id integer constraint PKLineItem primary key identity (1,1) not null, " +
    "OrderId integer not null, " +
    "ProductId integer not null, " +
    "Quantity integer default ((0)) not null, " +
    "Price decimal(11,2) default ((0)) not null, " +
    "AgeRequired integer default ((0)) not null, " +
    "StatusDate datetime2 not null, " +
    "Status integer default ((0)) not null, " +
    "constraint UniqueLineItem unique nonclustered (OrderId asc, ProductId asc), " +
    "constraint FKLineItemOrder foreign key (OrderId) references Orders(Id), " +
    "constraint FKLineItemProduct foreign key (ProductId) references Products(Id) " +
    ");";
}
