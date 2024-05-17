using Domain.SpectraData.Formats;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.Formats;

public class EspSpectraPlot(EspSpectra spectra, SignalXY plot) : SpectraPlot(spectra)
{
	public override string Name { get; protected set; } = spectra.Name;

	public override IEnumerable<IPlottable> GetPlottables()
	{
		yield return plot;
	}

	public override void ChangeColor(Color color)
	{
		PreviousColor = plot.Color;
		plot.Color = color;
	}
}