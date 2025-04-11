using System.Numerics;

namespace SpectraProcessing.Domain.Extensions;

public static class SpanExtensions
{
    public static T Sum<T>(this in Span<T> values) where T : struct, INumber<T>
    {
        T sum = default;

        foreach (var value in values)
        {
            sum += value;
        }

        return sum;
    }

    public static T Sum<T>(this in Span<T> values, Func<T, T> selector) where T : struct, INumber<T>
    {
        T sum = default;

        foreach (var value in values)
        {
            sum += selector(value);
        }

        return sum;
    }

    public static int Count<T>(this in Span<T> values, Func<T, bool> selector) where T : struct, INumber<T>
    {
        var count = 0;

        foreach (var value in values)
        {
            if (selector(value))
            {
                count++;
            }
        }

        return count;
    }
}
