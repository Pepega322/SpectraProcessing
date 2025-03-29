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
    IPeakDataPlotProvider peakDataPlotProvider
) : IProcessingController
{
    private static readonly OptimizationSettings OptimizationSettings = OptimizationSettings.Default;

    private SpectraKey? currentSpectraKey;

    private DataSet<PeakDataPlot> CurrentPeaksSet
        => currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false
            ? peaksStorage.DefaultSet
            : peaksStorage.Sets[currentSpectraKey];

    public IReadOnlyList<PeakDataPlot> CurrentPeaks => CurrentPeaksSet.Data;

    public event Action? OnPlotAreaChanged;

    public async Task AddPeak(PeakData peak)
    {
        var plot = (await peakDataPlotProvider.Draw([peak])).Single();

        CurrentPeaksSet.AddThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task RemovePeak(PeakData peak)
    {
        var plot = (await peakDataPlotProvider.Erase([peak])).Single();

        CurrentPeaksSet.RemoveThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();
    }

    public async Task ClearPeaks()
    {
        var peaks = CurrentPeaksSet.Data
            .Select(d => d.Peak)
            .ToArray();

        await peakDataPlotProvider.Erase(peaks);

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
                await peakDataPlotProvider.Clear();
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

        await peakDataPlotProvider.Clear();
        await DrawPeaksSet(peaksToDraw);
        OnPlotAreaChanged?.Invoke();

        return isCustom;
    }

    public async Task FitPeaks(IReadOnlyCollection<SpectraData> spectras)
    {
        foreach (var spectra in spectras)
        {
            await FitPeaksInternal(spectra);
        }

        OnPlotAreaChanged?.Invoke();

        return;

        async Task FitPeaksInternal(SpectraData spectra)
        {
            var key = new SpectraKey(spectra);

            PeakData[] currentPeaks;
            if (peaksStorage.Sets.TryGetValue(key, out var customPeaks))
            {
                currentPeaks = customPeaks.Data
                    .Select(d => d.Peak)
                    .ToArray();

                var fittedPeaks = await spectra.FitPeaks(
                    currentPeaks,
                    OptimizationSettings);

                for (var i = 0; i < fittedPeaks.Count; i++)
                {
                    customPeaks.Data[i].UpdatePeakEstimateData(fittedPeaks[i]);
                }
            }
            else
            {
                currentPeaks = peaksStorage.DefaultSet.Data
                    .Select(d => d.Peak.Copy())
                    .ToArray();

                var peaksPlots = await peakDataPlotProvider.GetPlots(currentPeaks);

                var fittedPeaks = await spectra.FitPeaks(
                    currentPeaks,
                    OptimizationSettings);

                for (var i = 0; i < fittedPeaks.Count; i++)
                {
                    peaksPlots[i].UpdatePeakEstimateData(fittedPeaks[i]);
                }

                await peaksStorage.AddDataSet(
                    key,
                    new DataSet<PeakDataPlot>(key.Name, peaksPlots));
            }
        }
    }

    public async Task<bool> SaveSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey))
        {
            return false;
        }

        await peakDataPlotProvider.Clear();

        var peaksCopy = peaksStorage.DefaultSet.Data
            .Select(d => d.Peak.Copy())
            .ToArray();

        var plotsCopy = await peakDataPlotProvider.Draw(peaksCopy);

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

        await peakDataPlotProvider.Clear();

        await DrawPeaksSet(peaksStorage.DefaultSet);

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    private Task DrawPeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var data = peaks.Data
            .Select(x => x.Peak)
            .ToArray();

        return peakDataPlotProvider.Draw(data);
    }
}
