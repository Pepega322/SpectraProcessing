namespace SpectraProcessing.Domain.Extensions;

public static class SetExtensions
{
    public static void AddRange<T>(this ISet<T> set, IReadOnlyCollection<T> items)
    {
        foreach (var item in items)
        {
            set.Add(item);
        }
    }

    public static void RemoveRange<T>(this ISet<T> set, IReadOnlyCollection<T> items)
    {
        foreach (var item in items)
        {
            set.Remove(item);
        }
    }
}
