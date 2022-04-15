using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum DalErrorCode
{
  [Description("No Error")]
  NoError = 0,
  [Description("Not found")]
  NotFound = 1,
  [Description("An exception occurred")]
  Exception = 2,
  [Description("Duplicate")]
  Duplicate = 3,
  [Description("Invalid parameter(s)")]
  InvalidParameters = 4,
  [Description("Not authenticated")]
  NotAuthenticated = 5,
  [Description("Not authorized")]
  NotAuthorized = 6
}
