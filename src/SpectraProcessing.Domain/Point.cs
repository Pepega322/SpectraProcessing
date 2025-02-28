using System.Numerics;

namespace SpectraProcessing.Domain;

public readonly struct Point<T>(T x, T y) where T : struct, INumber<T>
{
    public readonly T X = x;
    public readonly T Y = y;
}
