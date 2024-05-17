using Domain.Graphics;

namespace MathStatistics.SpectraProcessing;

public record PeakBorders : IPlottableData
{
	public readonly float XStart;
	public readonly float XEnd;

	public PeakBorders(float xStart, float xEnd)
	{
		if (xStart > xEnd)
			(xStart, xEnd) = (xEnd, xStart);

		XStart = xStart;
		XEnd = xEnd;
	}

	public override int GetHashCode() => HashCode.Combine(XStart, XEnd);

	public override string ToString() => $"{XStart: #.###} {XEnd: #.###}";
}