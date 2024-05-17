using Domain.SpectraData;

namespace Scott.Formats;

public abstract class SpectraPlot(Spectra spectra) : SctPlot
{
	public Spectra Spectra { get; init; } = spectra;
}