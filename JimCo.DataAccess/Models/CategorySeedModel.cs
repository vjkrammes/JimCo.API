namespace JimCo.DataAccess.Models;
public class CategorySeedModel
{
  public string Name { get; set; }
  public string Background { get; set; }
  public int AgeRequired { get; set; }
  public string Image { get; set; }

  public CategorySeedModel()
  {
    Name = string.Empty;
    Background = string.Empty;
    AgeRequired = 0;
    Image = string.Empty;
  }
}
