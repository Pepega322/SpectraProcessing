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
	PeakInfoSet ProcessPeaksForSingleSpectra(Spectra spectra);
	PeakInfoSet ProcessPeaksForSpectraSet(DataSet<Spectra> set);
	Spectra SubstractBaselineForSingleSpectra(Spectra spectra);
	DataSet<Spectra> SubstractBaselineForSingleSpectraSet(DataSet<Spectra> set);
	DataSet<Spectra> SubstractBaselineForSpectraSetFullDepth(DataSet<Spectra> set);
	Spectra GetAverageSpectraForSet(DataSet<Spectra> set);
}