using Domain.SpectraData.Processing;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.Formats;

public class PeakBorderPlot(PeakBorder border, VerticalLine leftLine, VerticalLine rightLine) : SctPlot
{
	public override string Name { get; protected set; } = border.ToString();

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