using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IProcessingController
{
    event Action? OnPlotAreaChanged;

    IReadOnlyList<PeakDataPlot> CurrentPeaks { get; }

    Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras);

    Task AddPeaks(IReadOnlyCollection<PeakData> peaks);

    Task RemovePeaks(IReadOnlyCollection<PeakData> peaks);

    Task<bool> CheckoutSpectra(SpectraData? spectra);

    Task FitPeaks(IReadOnlyCollection<SpectraData> spectras);

    Task<bool> SaveSpectraPeaks();

    Task<bool> RemovedSpectraPeaks();

    Task ClearPeaks();
}
