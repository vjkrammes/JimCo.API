namespace JimCo.Common;
public class AppSettings
{
  public string ApiBase { get; set; }
  public string ApiKey { get; set; }
  public string ImageBase { get; set; }
  public string ImageDirectory { get; set; }
  public string InternalApiKey { get; set; }
  public bool UpdateDatabase { get; set; }
  public string DefaultBackground { get; set; }

  public AppSettings()
  {
    ApiBase = string.Empty;
    ApiKey = string.Empty;
    ImageBase = string.Empty;
    ImageDirectory = string.Empty;
    InternalApiKey = string.Empty;
    UpdateDatabase = false;
    DefaultBackground = "White";
  }
}
