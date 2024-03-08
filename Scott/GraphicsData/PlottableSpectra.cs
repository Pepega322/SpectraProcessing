using Domain.SpectraData;
using ScottPlot;

namespace Scott.GraphicsData;
public abstract class PlottableSpectra(Spectra spectra) : Plottable {
    public Spectra Spectra { get; init; } = spectra;
    public IPlottable Plottable { get; protected set; } = null!;

    public override bool Equals(object? obj) {
        return obj is PlottableSpectra spectra && spectra.Spectra.Equals(Spectra);
    }

    public override int GetHashCode() => Spectra.GetHashCode();
}
