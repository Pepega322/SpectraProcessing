using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class ProcessingController(
    IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage,
    IDataPlotProvider<PeakData, PeakDataPlot> peaksPlotProvider
) : IProcessingController
{
    private SpectraKey? currentSpectraKey;

    private DataSet<PeakDataPlot> CurrentPeaks
        => currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false
            ? peaksStorage.DefaultSet
            : peaksStorage.Sets[currentSpectraKey];

    public event Action? OnPlotAreaChanged;

    public async Task AddPeak(PeakData peak)
    {
        var plot = (await peaksPlotProvider.Draw([peak])).Single();

        CurrentPeaks.AddThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task RemovePeak(PeakData peak)
    {
        var plot = (await peaksPlotProvider.Erase([peak])).Single();

        CurrentPeaks.RemoveThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task<bool> CheckoutSpectra(SpectraData? spectra)
    {
        if (spectra is null)
        {
            if (currentSpectraKey is not null && peaksStorage.Sets.ContainsKey(currentSpectraKey))
            {
                await peaksPlotProvider.Clear();
                await DrawPeaksSet(peaksStorage.DefaultSet);
                OnPlotAreaChanged?.Invoke();
            }

            currentSpectraKey = null;
            return false;
        }

        var key = new SpectraKey(spectra);

        if (key.Equals(currentSpectraKey))
        {
            return peaksStorage.Sets.ContainsKey(key);
        }

        currentSpectraKey = key;

        if (!peaksStorage.Sets.TryGetValue(key, out var customPeaks))
        {
            return false;
        }

        await peaksPlotProvider.Clear();
        await DrawPeaksSet(customPeaks);
        OnPlotAreaChanged?.Invoke();

        return true;
    }

    public async Task SaveSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey))
        {
            return;
        }

        var peaksCopy = peaksStorage.DefaultSet.Data
            .Select(d => d.Peak.Copy())
            .ToArray();

        await peaksPlotProvider.Clear();

        var plotsCopy = await peaksPlotProvider.Draw(peaksCopy);

        await peaksStorage.AddDataSet(currentSpectraKey, new DataSet<PeakDataPlot>(currentSpectraKey.Name, plotsCopy));

        OnPlotAreaChanged?.Invoke();
    }

    public async Task RemovedSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false)
        {
            return;
        }

        await peaksStorage.DeleteDataSet(currentSpectraKey);

        await peaksPlotProvider.Clear();

        await DrawPeaksSet(peaksStorage.DefaultSet);

        OnPlotAreaChanged?.Invoke();
    }

    private Task DrawPeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var data = peaks.Data
            .Select(x => x.Peak)
            .ToArray();

        return peaksPlotProvider.Draw(data);
    }
}
