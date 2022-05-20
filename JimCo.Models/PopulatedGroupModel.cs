namespace JimCo.Models;
public class PopulatedGroupModel
{
  public Guid Id { get; }
  public string Name { get; init; }
  public List<UserModel> Users { get; }

  public PopulatedGroupModel(string name)
  {
    Id = Guid.NewGuid();
    Name = name;
    Users = new List<UserModel>();
  }

  public PopulatedGroupModel(string name, List<UserModel> users) : this(name) => Users = new(users);
}
