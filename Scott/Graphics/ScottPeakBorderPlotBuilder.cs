using Domain.Graphics;
using Domain.SpectraData.Processing;
using Scott.Formats;
using ScottPlot;
using Plot = ScottPlot.Plot;

namespace Scott.Graphics;

public class ScottPeakBorderPlotBuilder(IPalette palette) : IPlotBuilder<PeakBorder, PeakBorderPlot>
{
	private readonly Plot builder = new();
	private readonly Dictionary<PeakBorder, PeakBorderPlot> plotted = [];
	private int counter;

	public PeakBorderPlot GetPlot(PeakBorder plottableData)
	{
		var color = palette.GetColor(counter);
		Interlocked.Increment(ref counter);

		if (plotted.TryGetValue(plottableData, out var plot))
		{
			plot.ChangeColor(color);
			return plot;
		}

		var left = builder.Add.VerticalLine(plottableData.Left, 1, color);
		var right = builder.Add.VerticalLine(plottableData.Right, 1, color);
		return new PeakBorderPlot(plottableData, left, right);
	}
}