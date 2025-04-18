using System.Numerics;

namespace SpectraProcessing.Domain.Extensions;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

    public static IEnumerable<T> Range<T>(T start, T delta, int count) where T : INumber<T>
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        for (var i = 0; i < count; i++)
        {
            yield return start;
            start += delta;
        }
    }

    public static Span<T> ToSpan<T>(this IEnumerable<T> enumerable, T[] buffer)
    {
        Span<T> span = new(buffer);

        using var enumerator = enumerable.GetEnumerator();

        span[0] = enumerator.Current;

        var index = 1;
        while (enumerator.MoveNext() && index < buffer.Length)
        {
            span[index] = enumerator.Current;
            index++;
        }

        return span[..index];
    }
}
