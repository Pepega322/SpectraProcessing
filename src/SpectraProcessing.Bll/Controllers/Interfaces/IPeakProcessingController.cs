using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IPeakProcessingController
{
    event Action? OnPlotAreaChanged;

    IReadOnlyList<PeakDataPlot> CurrentPeaks { get; }
    Task<bool> CheckoutSpectra(SpectraData? spectra);
    Task AddPeaksForCurrentSpectra(IReadOnlyCollection<PeakData> peaks);
    Task RemovePeaksForCurrentSpectra(IReadOnlyCollection<PeakData> peaks);
    Task<bool> SaveCurrentSpectraPeaks();
    Task<bool> RemoveCurrentSpectraPeaks();
    Task ClearCurrentSpectraPeaks();

    Task FitPeaks(IReadOnlyCollection<SpectraData> spectras);
    Task<IReadOnlyCollection<PeakData>> ExportPeaks(SpectraData spectra);
    Task ImportPeaks(SpectraData spectra, IReadOnlyCollection<PeakData> peaks);
    Task RemovePeaks(SpectraData spectra);
    Task ClearPeaks();
}
