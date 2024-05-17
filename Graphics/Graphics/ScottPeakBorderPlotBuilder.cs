using Domain.Graphics;
using MathStatistics.SpectraProcessing;
using Scott.Formats;
using ScottPlot;
using Plot = ScottPlot.Plot;

namespace Scott.Graphics;

public class ScottPeakBorderPlotBuilder(IPalette palette) : IPlotBuilder<PeakBorders, PeakBorderPlot>
{
	private readonly Plot builder = new();
	private readonly Dictionary<PeakBorders, PeakBorderPlot> plotted = [];
	private int counter;

	public PeakBorderPlot GetPlot(PeakBorders plottableData)
	{
		var color = palette.GetColor(counter);
		Interlocked.Increment(ref counter);

		if (plotted.TryGetValue(plottableData, out var plot))
		{
			plot.ChangeColor(color);
			return plot;
		}

		var left = builder.Add.VerticalLine(plottableData.XStart, 1, color);
		var right = builder.Add.VerticalLine(plottableData.XEnd, 1, color);
		return new PeakBorderPlot(plottableData, left, right);
	}
}