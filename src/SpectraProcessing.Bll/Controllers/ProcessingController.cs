using System.Collections.Immutable;
using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.MathModeling;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class ProcessingController(
    IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorage,
    IDataPlotProvider<PeakData, PeakDataPlot> peaksPlotProvider
) : IProcessingController
{
    private static readonly OptimizationSettings OptimizationSettings = OptimizationSettings.Default;

    private SpectraKey? currentSpectraKey;

    private DataSet<PeakDataPlot> CurrentPeaksSet
        => currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false
            ? peaksStorage.DefaultSet
            : peaksStorage.Sets[currentSpectraKey];

    public IImmutableSet<PeakDataPlot> CurrentPeaks => CurrentPeaksSet.Data;

    public event Action? OnPlotAreaChanged;

    public async Task AddPeak(PeakData peak)
    {
        var plot = (await peaksPlotProvider.Draw([peak])).Single();

        CurrentPeaksSet.AddThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task RemovePeak(PeakData peak)
    {
        var plot = (await peaksPlotProvider.Erase([peak])).Single();

        CurrentPeaksSet.RemoveThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ClearPeaks()
    {
        var peaks = CurrentPeaksSet.Data
            .Select(d => d.Peak)
            .ToArray();

        await peaksPlotProvider.Erase(peaks);

        CurrentPeaksSet.ClearThreadSafe();

        OnPlotAreaChanged?.Invoke();
    }

    public async Task<bool> CheckoutSpectra(SpectraData? spectra)
    {
        var previousKey = currentSpectraKey;

        if (spectra is null)
        {
            if (previousKey is not null && peaksStorage.Sets.ContainsKey(previousKey))
            {
                await peaksPlotProvider.Clear();
                await DrawPeaksSet(peaksStorage.DefaultSet);
                OnPlotAreaChanged?.Invoke();
            }

            currentSpectraKey = null;
            return false;
        }

        var newKey = new SpectraKey(spectra);

        if (newKey.Equals(previousKey))
        {
            return peaksStorage.Sets.ContainsKey(newKey);
        }

        currentSpectraKey = newKey;

        var (isCustom, peaksToDraw) = peaksStorage.Sets.TryGetValue(newKey, out var customPeaks)
            ? (true, customPeaks)
            : (false, peaksStorage.DefaultSet);

        await peaksPlotProvider.Clear();
        await DrawPeaksSet(peaksToDraw);
        OnPlotAreaChanged?.Invoke();

        return isCustom;
    }

    public async Task<bool> SaveSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey))
        {
            return false;
        }

        await peaksPlotProvider.Clear();

        var peaksCopy = peaksStorage.DefaultSet.Data
            .Select(d => d.Peak.Copy())
            .ToArray();

        var plotsCopy = await peaksPlotProvider.Draw(peaksCopy);

        await peaksStorage.AddDataSet(currentSpectraKey, new DataSet<PeakDataPlot>(currentSpectraKey.Name, plotsCopy));

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    public async Task<bool> RemovedSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false)
        {
            return false;
        }

        await peaksStorage.DeleteDataSet(currentSpectraKey);

        await peaksPlotProvider.Clear();

        await DrawPeaksSet(peaksStorage.DefaultSet);

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    private Task DrawPeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var data = peaks.Data
            .Select(x => x.Peak)
            .ToArray();

        return peaksPlotProvider.Draw(data);
    }
}
