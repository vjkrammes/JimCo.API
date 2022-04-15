using System.ComponentModel;

namespace JimCo.Common.Enumerations;
public enum AlertLevel
{
  [Description("Information")]
  Information = 0,
  [Description("Notice")]
  Notice = 1,
  [Description("Critical")]
  Critical = 2
}
