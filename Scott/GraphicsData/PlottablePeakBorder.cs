using Domain.SpectraData;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.GraphicsData;

public class PlottablePeakBorder : Plottable
{
	public VerticalLine LeftLine { get; init; }
	public VerticalLine RightLine { get; init; }

	public PlottablePeakBorder(PeakBorder border)
	{
		lock (Builder)
		{
			LeftLine = Builder.Add.VerticalLine(border.Left, 1);
			RightLine = Builder.Add.VerticalLine(border.Rigth, 1);
		}
	}

	public override void SetColor(Color color)
	{
		Color = color;
		LeftLine.Color = color;
		RightLine.Color = color;
	}

	public override IEnumerable<IPlottable> GetPlots()
	{
		yield return LeftLine;
		yield return RightLine;
	}

	public override bool Equals(object? obj)
	{
		return obj is PlottablePeakBorder border && LeftLine.X == border.LeftLine.X &&
			RightLine.X == border.RightLine.X;
	}

	public override int GetHashCode() => HashCode.Combine(LeftLine.X, RightLine.X);
}