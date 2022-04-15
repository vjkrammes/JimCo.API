namespace JimCo.Common;
public class ItemRange<T> where T : IComparable<T>
{
  public T Minimum { get; }
  public T Maximum { get; }

  public ItemRange(T min, T max)
  {
    Minimum = min;
    Maximum = max;
  }

  public bool Overlaps(ItemRange<T> other) => Minimum.CompareTo(other.Maximum) <= 0 && Maximum.CompareTo(other.Minimum) >= 0;
}
