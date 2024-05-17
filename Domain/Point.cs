using System.Numerics;

namespace Domain;

public struct Point<T>(T x, T y) where T : struct, INumber<T>
{
	public readonly T X = x;
	public readonly T Y = y;
}