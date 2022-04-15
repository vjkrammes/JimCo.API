namespace JimCo.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BuildOrderAttribute : Attribute
{
  public int BuildOrder { get; }
  public BuildOrderAttribute() => BuildOrder = 0;
  public BuildOrderAttribute(int order) => BuildOrder = order;
}
