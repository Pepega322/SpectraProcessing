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
}
