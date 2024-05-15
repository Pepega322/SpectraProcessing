using Domain.SpectraData;
using ScottPlot;
using ScottPlot.Plottables;

namespace Scott.GraphicsData;

internal class PlottableASP : PlottableSpectra
{
	public PlottableASP(Asp spectra) : base(spectra)
	{
		lock (Builder)
			Plottable = Builder.Add.Signal(spectra.Points.Y.ToArray(), spectra.Info.Delta);
	}

	public override IEnumerable<IPlottable> GetPlots()
	{
		yield return Plottable;
	}

	public override void SetColor(Color color)
	{
		Color = color;
		((Signal) Plottable).Color = color;
	}
}