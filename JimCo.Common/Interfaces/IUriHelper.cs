namespace JimCo.Common.Interfaces;
public interface IUriHelper
{
  void SetBase(string path);
  void SetVersion(int version);
  Uri Create(string? controller = null, string? action = null, params object[] parms);
}
