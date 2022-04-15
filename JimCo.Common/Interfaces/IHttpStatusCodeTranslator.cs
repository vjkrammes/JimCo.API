using System.Net;

namespace JimCo.Common.Interfaces;
public interface IHttpStatusCodeTranslator
{
  string Translate(int code);
  string Translate(HttpStatusCode code);
}
