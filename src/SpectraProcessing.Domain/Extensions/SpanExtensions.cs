using System.Numerics;

namespace SpectraProcessing.Domain.Extensions;

public static class SpanExtensions
{
    public static T Sum<T>(this ref Span<T> values) where T : struct, INumber<T>
    {
        T sum = default;

        foreach (var value in values)
        {
            sum += value;
        }

        return sum;
    }

    public static T Sum<T>(this ref Span<T> values, Func<T, T> selector) where T : struct, INumber<T>
    {
        T sum = default;

        foreach (var value in values)
        {
            sum += selector(value);
        }

        return sum;
    }
}
