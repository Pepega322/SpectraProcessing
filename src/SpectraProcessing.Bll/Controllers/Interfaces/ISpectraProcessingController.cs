using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers.Interfaces;

public interface ISpectraProcessingController
{
    event Action? OnPlotAreaChanged;

    Task AddPeak(PeakData peak);

    Task RemovePeak(PeakData peak);

    Task SaveSpectraPeaks(SpectraData spectra);
}
