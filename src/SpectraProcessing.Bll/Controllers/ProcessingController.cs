using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Extensions;
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
    private static readonly OptimizationSettings OptimizationSettings = OptimizationSettings.Default
        with
        {
            MaxIterationsCount = 1000,
            Criteria = new OptimizationSettings.CompletionСriteria
            {
                AbsoluteValue = null,
                // MaxConsecutiveShrinks = 50,
            },
        };

    private SpectraKey? currentSpectraKey;

    private DataSet<PeakDataPlot> CurrentPeaksSet
        => currentSpectraKey is null || peaksStorage.Sets.ContainsKey(currentSpectraKey) is false
            ? peaksStorage.DefaultSet
            : peaksStorage.Sets[currentSpectraKey];

    public IReadOnlyList<PeakDataPlot> CurrentPeaks => CurrentPeaksSet.Data;

    public event Action? OnPlotAreaChanged;

    public async Task SmoothSpectras(IReadOnlyCollection<SpectraData> spectras)
    {
        var smoothingTasks = spectras.Select(s => Task.Run(() => s.Points.Smooth()));

        await Task.WhenAll(smoothingTasks);

        OnPlotAreaChanged?.Invoke();
    }

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

        await CheckoutSpectra(spectras.Last());
        OnPlotAreaChanged?.Invoke();

        return;

        async Task FitPeaksInternal(SpectraData spectra)
        {
            var key = new SpectraKey(spectra);

            if (peaksStorage.Sets.TryGetValue(key, out var customPeaks))
            {
                var peaks = customPeaks!.Data
                    .Select(d => d.Peak)
                    .ToArray();

                if (peaks.IsEmpty())
                {
                    return;
                }

                var plots = customPeaks.Data;

                await spectra.FitPeaks(peaks, OptimizationSettings);

                foreach (var peakPlot in plots)
                {
                    peakPlot.UpdateMarkers();
                }
            }
            else
            {
                var peaks = peaksStorage.DefaultSet.Data
                    .Select(d => d.Peak.Copy())
                    .ToArray();

                if (peaks.IsEmpty())
                {
                    return;
                }

                var plots = await peakDataPlotProvider.GetPlots(peaks);

                await spectra.FitPeaks(peaks, OptimizationSettings);

                foreach (var peakPlot in plots)
                {
                    peakPlot.UpdateMarkers();
                }

                await peaksStorage.AddDataSet(key, new DataSet<PeakDataPlot>(key.Name, plots));
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
