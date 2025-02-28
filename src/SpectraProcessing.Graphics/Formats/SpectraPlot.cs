using SpectraProcessing.Domain.SpectraData;

namespace SpectraProcessing.Graphics.Formats;

public abstract class SpectraPlot(Spectra spectra) : SctPlot
{
    public Spectra Spectra { get; init; } = spectra;
}
