using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using JimCo.Common.Enumerations;

namespace JimCo.Common;
public static class ExtensionMethods
{
  public static bool IsStrongPassword(this PasswordStrength strength) => strength is PasswordStrength.Strong or PasswordStrength.VeryStrong;

  public static string FirstWord(this string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }
    return value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).First();
  }

  public static string Host(this string uri)
  {
    if (string.IsNullOrWhiteSpace(uri))
    {
      return string.Empty;
    }
    try
    {
      var u = new Uri(uri);
      return u.Host;
    }
    catch
    {
      return string.Empty;
    }
  }

  public static int Sign<T>(this T value) where T : IComparable<T>
  {
    if (value.CompareTo(default) < 0)
    {
      return -1;
    }
    if (value.CompareTo(default) > 0)
    {
      return 1;
    }
    return 0;
  }

  public static string StripPadding(this string value) => value.TrimEnd('=');

  public static string AddPadding(this string value) => (value.Length % 4) switch
  {
    0 => value,
    1 => value + "===",
    2 => value + "==",
    _ => value + "="
  };

  public static byte[] ComputeHash(this byte[] value, string key, string hash = "SHA512") => value.ComputeHash(Encoding.UTF8.GetBytes(key), hash);

  public static byte[] ComputeHash(this byte[] value, byte[] key, string hash = "SHA512")
  {
    byte[] signature;
    switch (hash.ToLower(CultureInfo.CurrentCulture))
    {
      case "sha256":
        using (var hmac = new HMACSHA256(key))
        {
          signature = hmac.ComputeHash(value);
        }
        break;
      case "sha384":
        using (var hmac = new HMACSHA384(key))
        {
          signature = hmac.ComputeHash(value);
        }
        break;
      case "sha512":
        using (var hmac = new HMACSHA512(key))
        {
          signature = hmac.ComputeHash(value);
        }
        break;
      default:
        throw new ArgumentException($"Only SHA256, SHA384 and SHA512 are supported, found '{hash}'", nameof(hash));
    }
    return signature;
  }

  public static (byte o0, byte o1, byte o2, byte o3) Octets(this ulong value)
  {
    var o0 = (byte)((value >> 24) & 0xff);
    var o1 = (byte)((value >> 16) & 0xff);
    var o2 = (byte)((value >> 8) & 0xff);
    var o3 = (byte)(value & 0xff);
    return (o0, o1, o2, o3);
  }

  public static (byte o0, byte o1, byte o2, byte o3) Octets(this long value) => ((ulong)value).Octets();

  public static (byte o0, byte o1, byte o2, byte o3) Octets(this uint value) => ((ulong)value).Octets();

  public static (byte o0, byte o1, byte o2, byte o3) Octets(this int value) => ((ulong)value).Octets();

  public static string Hexify(this ulong argb)
  {
    StringBuilder sb = new("0x");
    var (o0, o1, o2, o3) = argb.Octets();
    sb.Append(o0.ToString("x2"));
    sb.Append(o1.ToString("x2"));
    sb.Append(o2.ToString("x2"));
    sb.Append(o3.ToString("x2"));
    return sb.ToString();
  }

  public static string Hexify(this long argb) => ((ulong)argb).Hexify();

  public static string Hexify(this uint argb) => ((ulong)argb).Hexify();

  public static string Hexify(this int argb) => ((ulong)argb).Hexify();

  public static string Hexify(this byte[] value, bool addHeader = true)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }
    if (!value.Any())
    {
      return string.Empty;
    }
    var sb = new StringBuilder();
    if (addHeader)
    {
      sb.Append("0x");
    }
    foreach (var b in value)
    {
      sb.Append(b.ToString("x2"));
    }
    return sb.ToString();
  }

  public static IEnumerable<TModel> ToModels<TModel, TEntity>(this IEnumerable<TEntity> entities, string methodName = "FromEntity")
    where TModel : class where TEntity : class
  {
    var ret = new List<TModel>();
    var method = typeof(TModel).GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null,
      new[] { typeof(TEntity) }, Array.Empty<ParameterModifier>());
    if (method is not null)
    {
      if (entities is not null && entities.Any())
      {
        entities.ForEach(x => ret.Add((method.Invoke(null, new[] { x }) as TModel)!));
      }
    }
    return ret;
  }

  public static string TrimEnd(this string value, string suffix, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
  {
    if (value is null)
    {
      throw new ArgumentNullException(nameof(value));
    }
    if (!string.IsNullOrWhiteSpace(suffix) && value.EndsWith(suffix, comparer))
    {
      return value[..^suffix.Length];
    }
    return value;
  }

  public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
  {
    if (list is null || action is null)
    {
      return;
    }
    foreach (var item in list)
    {
      action(item);
    }
  }

  public static string Beginning(this string value, int length, char ellipsis = '.')
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }
    return value.Length <= length ? value : value[..length] + new string(ellipsis, 3);
  }

  public static T[] ArrayCopy<T>(this T[] source)
  {
    if (source is null)
    {
      return Array.Empty<T>();
    }
    var ret = new T[source.Length];
    Array.Copy(source, ret, source.Length);
    return ret;
  }

  public static bool ArrayEquals<T>(this T[] left, T[] right)
  {
    if (left is null)
    {
      if (right is null)
      {
        return true;
      }
      return false;
    }
    if (right is null)
    {
      return false;
    }
    if (ReferenceEquals(left, right))
    {
      return true;
    }
    if (left.Length != right.Length)
    {
      return false;
    }
    var comparer = EqualityComparer<T>.Default;
    for (var i = 0; i < left.Length; i++)
    {
      if (!comparer.Equals(right[i], left[i]))
      {
        return false;
      }
    }
    return true;
  }

  public static T[] Append<T>(this T[] left, T[] right)
  {
    if (left is null || !left.Any())
    {
      return right ?? Array.Empty<T>();
    }
    if (right is null || !right.Any())
    {
      return left;
    }
    var ret = new T[left.Length + right.Length];
    Array.Copy(left, 0, ret, 0, left.Length);
    Array.Copy(right, 0, ret, left.Length, right.Length);
    return ret;
  }

  public static string Capitalize(this string value, CultureInfo? culture = null)
  {
    if (culture is null)
    {
      culture = CultureInfo.CurrentCulture;
    }
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }
    return value.First().ToString().ToUpper(culture) + string.Join(string.Empty, value.Skip(1));
  }

  public static string GetDescriptionFromEnumValue<T>(this T value) where T : Enum =>
    typeof(T)
      .GetField(value.ToString())!
      .GetCustomAttributes(typeof(DescriptionAttribute), false)
      .SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;

  public static string Innermost(this Exception exception)
  {
    if (exception is null)
    {
      throw new ArgumentNullException(nameof(exception));
    }
    if (exception.InnerException is null)
    {
      return exception.Message;
    }
    return exception.InnerException.Innermost();
  }

  public static bool IsBetween(DateTime date, DateTime start, DateTime end) => date >= start && date <= end;
}
