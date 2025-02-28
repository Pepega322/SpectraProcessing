using SpectraProcessing.Domain.SpectraData;
using SpectraProcessing.Domain.Storage;
using SpectraProcessing.MathStatistics.SpectraProcessing;

namespace SpectraProcessing.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    IEnumerable<PeakBorders> Borders { get; }
    void AddBorder(PeakBorders peakBorders);
    void RemoveBorder(PeakBorders peakBorders);
    void ClearBorders();
    void RedrawBorders();
    Task ImportBorders(string fullname);
    Task<SpectrasProcessingResult> ProcessPeaksForSingleSpectra(Spectra spectra);
    Task<SpectrasProcessingResult> ProcessPeaksForSpectraSet(DataSet<Spectra> set);
    Task<Spectra> SubstractBaseline(Spectra spectra);
    Task<Spectra[]> SubstractBaseline(IEnumerable<Spectra> set);
    Task<Spectra> GetAverageSpectra(IEnumerable<Spectra> set);
}
