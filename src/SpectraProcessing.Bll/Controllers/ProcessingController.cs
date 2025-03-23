using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class ProcessingController(IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage)
    : IProcessingController
{
    public event Action? OnPlotAreaChanged;

    public Task AddPeak(PeakData peak)
    {
        //TODO работаем тут
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
