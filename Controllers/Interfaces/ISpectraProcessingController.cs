using Domain.SpectraData;
using Domain.SpectraData.Processing;
using Domain.Storage;

namespace Controllers.Interfaces;

public interface ISpectraProcessingController
{
	IEnumerable<PeakBorder> Borders { get; }
	void AddBorder(PeakBorder border);
	void RemoveBorder(PeakBorder border);
	void ClearBorders();
	void RedrawBorders();
	Task<PeakInfoSet> ProcessPeaksForSingleSpectra(Spectra spectra);
	Task<PeakInfoSet> ProcessPeaksForSpectraSet(DataSet<Spectra> set);
	Task<Spectra> SubstractBaseline(Spectra spectra);
	Task<Spectra[]> SubstractBaseline(IEnumerable<Spectra> set);
	Task<Spectra> GetAverageSpectra(IEnumerable<Spectra> set);
}