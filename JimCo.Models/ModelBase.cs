namespace JimCo.Models;
public abstract class ModelBase
{
  public bool CanDelete { get; set; }

  public ModelBase() => CanDelete = true;

  public ModelBase(bool canDelete) => CanDelete = canDelete;
}
