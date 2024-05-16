using Domain.Graphics;

namespace Domain.SpectraData.Processing;

public class PeakBorder : IPlottableData
{
	public float Left { get; init; }
	public float Right { get; init; }

	public PeakBorder(float left, float right)
	{
		if (left > right)
		{
			(left, right) = (right, left);
		}

		Left = left;
		Right = right;
	}

	public override bool Equals(object? obj)
	{
		return obj is PeakBorder border && Math.Abs(Left - border.Left) < 1e-6 && Math.Abs(Right - border.Right) < 1e-6;
	}

	public override int GetHashCode() => HashCode.Combine(Left, Right);

	public override string ToString() => $"{Left: #.###}:{Right: #.###}";
}