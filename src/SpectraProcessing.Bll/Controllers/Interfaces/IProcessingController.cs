using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface IProcessingController
{
    event Action? OnPlotAreaChanged;

    Task AddPeak(PeakData peak);

    Task RemovePeak(PeakData peak);

    Task<bool> CheckoutSpectra(SpectraData? spectra);

    Task SaveSpectraPeaks();

    Task RemovedSpectraPeaks();
}
