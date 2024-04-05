using Domain.SpectraData;
using ScottPlot;

namespace Scott.GraphicsData;

public abstract class PlottableSpectra(Spectra spectra) : Plottable {
	private Spectra Spectra { get; init; } = spectra;
	protected IPlottable Plottable { get; init; } = null!;

	public override bool Equals(object? obj) {
		return obj is PlottableSpectra spectra && spectra.Spectra.Equals(Spectra);
	}

	public override int GetHashCode() => Spectra.GetHashCode();
}