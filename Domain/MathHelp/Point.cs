using System.Numerics;

namespace Domain.MathHelp;
public struct Point<T> where T : struct, INumber<T>
{
    public readonly T X;
    public readonly T Y;

    public Point(T x, T y)
    {
        X = x;
        Y = y;
    }
}
