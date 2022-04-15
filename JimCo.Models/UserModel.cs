using System.Globalization;

using JimCo.Common;
using JimCo.DataAccess.Entities;

namespace JimCo.Models;

public class UserModel : ModelBase, IEquatable<UserModel>, IFormattable
{
  public string Id { get; set; }
  public string Identifier { get; set; }
  public string Email { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string DisplayName { get; set; }
  public DateTime DateJoined { get; set; }
  public string JobTitles { get; set; }

  public UserModel() : base(true)
  {
    Id = string.Empty;
    Identifier = string.Empty;
    Email = string.Empty;
    FirstName = string.Empty;
    LastName = string.Empty;
    DisplayName = string.Empty;
    DateJoined = default;
    JobTitles = string.Empty;
  }

  public static UserModel? FromEntity(UserEntity entity) => entity is null ? null : new()
  {
    Id = IdEncoder.EncodeId( entity.Id),
    Identifier = entity.Identifier ?? string.Empty,
    Email = entity.Email ?? string.Empty,
    FirstName = entity.FirstName ?? string.Empty,
    LastName = entity.LastName ?? string.Empty,
    DisplayName = entity.DisplayName ?? string.Empty,
    DateJoined = entity.DateJoined,
    JobTitles = entity.JobTitles ?? string.Empty,
    CanDelete = true
  };

  public static UserEntity? FromModel(UserModel model) => model is null ? null : new()
  {
    Id = IdEncoder.DecodeId(model.Id),
    Identifier = model.Identifier ?? string.Empty,
    Email = model.Email ?? string.Empty,
    FirstName = model.FirstName ?? string.Empty,
    LastName = model.LastName ?? string.Empty,
    DisplayName = model.DisplayName ?? string.Empty,
    DateJoined = model.DateJoined,
    JobTitles = model.JobTitles ?? string.Empty
  };

  public UserModel Clone() => new()
  {
    Id = Id,
    Identifier = Identifier ?? string.Empty,
    Email = Email ?? string.Empty,
    FirstName = FirstName ?? string.Empty,
    LastName = LastName ?? string.Empty,
    DisplayName = DisplayName ?? string.Empty,
    DateJoined = DateJoined,
    JobTitles = JobTitles ?? string.Empty,
    CanDelete = CanDelete
  };

  private string DefaultName() => ToString("fl");

  public override string ToString() => DefaultName();

  public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

  public string ToString(string? format, IFormatProvider? provider)
  {
    if (string.IsNullOrWhiteSpace(format))
    {
      return DefaultName();
    }
    var culture = provider is null ? CultureInfo.CurrentCulture : (CultureInfo)provider;
    var ret = format.ToLower(culture) switch
    {
      "fl" => string.IsNullOrWhiteSpace(FirstName) ? LastName : $"{FirstName} {LastName}",
      "lf" => string.IsNullOrWhiteSpace(FirstName) ? LastName : $"{LastName}, {FirstName}",
      "d" => string.IsNullOrWhiteSpace(DisplayName) ? DefaultName() : DisplayName,
      "e" => string.IsNullOrWhiteSpace(Email) ? DefaultName() : Email,
      "extf" => $"{ToString("fl")} ({Email})",
      "extl" => $"{ToString("lf")} ({Email})",
      "extd" => $"{ToString("d")} ({Email})",
      _ => DefaultName()
    };
    return ret;
  }

  public override bool Equals(object? obj) => obj is UserModel model && model.Id == Id;

  public bool Equals(UserModel? model) => model is not null && model.Id == Id;

  public override int GetHashCode() => Id.GetHashCode();

  public static bool operator ==(UserModel left, UserModel right) => (left, right) switch
  {
    (null, null) => true,
    (null, _) or (_, null) => false,
    (_, _) => left.Id == right.Id
  };

  public static bool operator !=(UserModel left, UserModel right) => !(left == right);

  public static implicit operator UserModel?(UserEntity entity) => FromEntity(entity);

  public static implicit operator UserEntity?(UserModel model) => FromModel(model);
}
