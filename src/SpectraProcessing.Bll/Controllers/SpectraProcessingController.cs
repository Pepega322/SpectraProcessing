using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

public class SpectraProcessingController(IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage)
    : ISpectraProcessingController
{
    public event Action? OnPlotAreaChanged;

    public Task AddPeak(PeakData peak)
    {
        throw new NotImplementedException();
    }

    public Task RemovePeak(PeakData peak)
    {
        throw new NotImplementedException();
    }

    public Task SaveSpectraPeaks(SpectraData spectra)
    {
        throw new NotImplementedException();
    }
}
