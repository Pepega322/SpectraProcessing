using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IProcessingController
{
    event Action? OnPlotAreaChanged;

    IReadOnlyList<PeakDataPlot> CurrentPeaks { get; }

    Task AddPeak(PeakData peak);

    Task RemovePeak(PeakData peak);

    Task<bool> CheckoutSpectra(SpectraData? spectra);

    Task<bool> SaveSpectraPeaks();

    Task<bool> RemovedSpectraPeaks();

    Task ClearPeaks(); }
