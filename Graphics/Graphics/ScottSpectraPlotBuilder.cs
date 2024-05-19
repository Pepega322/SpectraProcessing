using System.Collections.Concurrent;
using Domain.Graphics;
using Domain.SpectraData;
using Domain.SpectraData.Formats;
using Scott.Formats;
using ScottPlot;
using PlotArea = ScottPlot.Plot;

namespace Scott.Graphics;

public class ScottSpectraPlotBuilder(IPalette palette) : IPlotBuilder<Spectra, SpectraPlot>
{
	private readonly PlotArea builder = new();
	private readonly ConcurrentDictionary<Spectra, SpectraPlot> plotted = [];
	private int counter;

	public SpectraPlot GetPlot(Spectra plottableData)
	{
		var color = palette.GetColor(counter);
		Interlocked.Increment(ref counter);

		if (plotted.TryGetValue(plottableData, out var plot))
		{
			plot.ChangeColor(color);
			return plot;
		}

		SpectraPlot result;
		switch (plottableData)
		{
			case AspSpectra asp:
			{
				var aspPlot = builder.Add.Signal(asp.Points.Y.ToArray(), asp.Info.Delta, color);
				result = new AspSpectraPlot(asp, aspPlot);
				break;
			}
			case EspSpectra esp:
			{
				var espPlot = builder.Add.SignalXY(esp.Points.X.ToArray(), esp.Points.Y.ToArray(), color);
				result = new EspSpectraPlot(esp, espPlot);
				break;
			}
			default: throw new NotSupportedException();
		}

		plotted.TryAdd(plottableData, result);
		return result;
	}
}