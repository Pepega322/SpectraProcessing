using System.Numerics;

namespace SpectraProcessing.Models.Collections;

public class Point<T>(T x, T y) where T : struct, INumber<T>
{
    public T X { get; set; } = x;
    public T Y { get; set; } = y;
}
