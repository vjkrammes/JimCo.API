namespace JimCo.Common.Interfaces;
public interface ICacheService
{
  T Get<T>(string key) where T : class;
  void Set<T>(string key, T item, int expirationMinutes) where T : class;
  void Set<T>(string key, T item, TimeSpan expiration) where T : class;
  bool IsCached(string key);
  bool IsCachedItem<T>(string key) where T : class;
  void RemoveItem(string key);
  void ClearCache();
  void ClearCacheExcept(params string[] keys);
  IEnumerable<string> Keys { get; }
}
