using Domain.SpectraData;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.GraphicsData;

internal class PlottableESP : PlottableSpectra
{
	public PlottableESP(Esp spectra) : base(spectra)
	{
		lock (Builder)
			Plottable = Builder.Add.SignalXY(spectra.Points.X.ToArray(), spectra.Points.Y.ToArray());
	}

	public override IEnumerable<IPlottable> GetPlots()
	{
		yield return Plottable;
	}

	public override void SetColor(Color color)
	{
		Color = color;
		((SignalXY) Plottable).Color = color;
	}
}