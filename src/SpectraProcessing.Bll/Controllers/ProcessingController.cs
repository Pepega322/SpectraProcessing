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
    public static readonly OptimizationSettings OptimizationSettings = OptimizationSettings.Default
        with
        {
            RepeatsCount = 3,
            MaxIterationsCount = 1000,
            Criteria = new OptimizationSettings.CompletionСriteria
            {
                AbsoluteValue = 1e-3f,
                MaxConsecutiveShrinks = 50,
                MinDeltaBetweenBetweenIterations = 1e-6f,
                MaxIterationsWithLessThanDelta = 50,
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

    public Task AddPeak(PeakData peak)
    {
        var plot = peakDataPlotProvider.Draw([peak]).Single();

        CurrentPeaksSet.AddThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task RemovePeak(PeakData peak)
    {
        var plot = peakDataPlotProvider.Erase([peak]).Single();

        CurrentPeaksSet.RemoveThreadSafe(plot);

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task ClearPeaks()
    {
        peakDataPlotProvider.Clear();

        CurrentPeaksSet.ClearThreadSafe();

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task<bool> CheckoutSpectra(SpectraData? spectra)
    {
        var previousKey = currentSpectraKey;

        if (spectra is null)
        {
            if (previousKey is not null && peaksStorage.Sets.TryGetValue(previousKey, out var plotSet))
            {
                peakDataPlotProvider.Clear();
                DrawPeaksSet(peaksStorage.DefaultSet);
                OnPlotAreaChanged?.Invoke();
            }

            currentSpectraKey = null;
            return Task.FromResult(false);
        }

        var newKey = new SpectraKey(spectra);

        if (previousKey is not null && newKey.Equals(previousKey))
        {
            return Task.FromResult(peaksStorage.Sets.ContainsKey(newKey));
        }

        var (isCustom, peaksToDraw) = peaksStorage.Sets.TryGetValue(newKey, out var customPeaks)
            ? (true, customPeaks)
            : (false, peaksStorage.DefaultSet);

        peakDataPlotProvider.Clear();
        DrawPeaksSet(peaksToDraw);
        currentSpectraKey = newKey;
        OnPlotAreaChanged?.Invoke();

        return Task.FromResult(isCustom);
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

            if (peaksStorage.Sets.TryGetValue(key, out var customPeaks))
            {
                if (customPeaks!.Data.IsEmpty())
                {
                    return;
                }

                var peaks = customPeaks!.Data
                    .Select(d => d.Peak);

                await spectra.FitPeaks(peaks, OptimizationSettings);

                foreach (var plot in customPeaks.Data)
                {
                    plot.UpdateMarkers();
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

                var plots = peakDataPlotProvider.GetPlots(peaks).ToArray();

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

        peakDataPlotProvider.Clear();

        var peaksCopy = peaksStorage.DefaultSet.Data
            .Select(d => d.Peak.Copy())
            .ToArray();

        var plotsCopy = peakDataPlotProvider.Draw(peaksCopy).ToArray();

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

        peakDataPlotProvider.Clear();

        DrawPeaksSet(peaksStorage.DefaultSet);

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    private void DrawPeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var peakData = peaks.Data
            .Select(x => x.Peak);

        peakDataPlotProvider.Draw(peakData);
    }

    private void ErasePeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var peakData = peaks.Data
            .Select(x => x.Peak);

        peakDataPlotProvider.Erase(peakData);
    }
}
