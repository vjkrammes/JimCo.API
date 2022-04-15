using System.ComponentModel.DataAnnotations;

namespace JimCo.Common.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class NonNegativeAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value is null)
    {
      return new("Value is null");
    }
    if (validationContext is null)
    {
      return new("Validation context is null");
    }
    var valid = false;
    switch (value)
    {
      case int ival:
        valid = ival >= 0;
        break;
      case long lval:
        valid = lval >= 0;
        break;
      case float fval:
        valid = fval >= 0.0;
        break;
      case double dval:
        valid = dval >= 0.0;
        break;
      case decimal mval:
        valid = mval >= 0M;
        break;

    }
    if (valid)
    {
      return null;
    }
    if (string.IsNullOrWhiteSpace(ErrorMessage))
    {
      var prop = validationContext.DisplayName;
      return new($"{prop} must be a number greater than or equal to zero");
    }
    return new(ErrorMessage);
  }
}
