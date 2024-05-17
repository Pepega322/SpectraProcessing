using MathStatistics.SpectraProcessing;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.Formats;

public class PeakBorderPlot(PeakBorders borders, VerticalLine leftLine, VerticalLine rightLine) : SctPlot
{
	public override string Name { get; protected set; } = borders.ToString();

	public override IEnumerable<IPlottable> GetPlottables()
	{
		yield return leftLine;
		yield return rightLine;
	}

	public override void ChangeColor(Color color)
	{
		PreviousColor = leftLine.Color;
		rightLine.Color = color;
		leftLine.Color = color;
	}
}