using SpectraProcessing.Domain.SpectraData;

namespace SpectraProcessing.MathStatistics.SpectraProcessing;

public class SpectraPeak(Spectra spectra, PeakBorders borders, float square, float height)
{
    public string SpectraName => spectra.Name;
    public readonly PeakBorders Borders = borders;
    public readonly float Square = square;
    public readonly float Height = height;
}
