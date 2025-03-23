using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Models.Collections.Keys;
using SpectraProcessing.Models.Peak;
using SpectraProcessing.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

public class ProcessingController(IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage)
    : IProcessingController
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
