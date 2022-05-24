using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum Level
{
  [Description("No Level")]
  NoLevel = 0,
  [Description("Debug")]
  Debug = 1,
  [Description("Informational")]
  Information = 2,
  [Description("Warning")]
  Warning = 3,
  [Description("Error")]
  Error = 4,
  [Description("Critical")]
  Critical = 5,
  [Description("Fatal")]
  Fatal = 6
}
