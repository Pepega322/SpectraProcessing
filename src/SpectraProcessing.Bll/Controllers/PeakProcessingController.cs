using SpectraProcessing.Bll.Controllers.Interfaces;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Monitors.Interfaces;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Collections;
using SpectraProcessing.Domain.Collections.Keys;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.MathModeling.Peaks;
using SpectraProcessing.Domain.Models.Peak;
using SpectraProcessing.Domain.Models.Spectra.Abstractions;

namespace SpectraProcessing.Bll.Controllers;

internal sealed class PeakProcessingController(
    IDataStorageProvider<SpectraKey, PeakDataPlot> peaksStorageProvider,
    IPeakDataPlotProvider peakDataPlotProvider,
    IPeakProcessingSettingsMonitor peakProcessingSettingsMonitor
) : IPeakProcessingController
{
    private SpectraKey? currentSpectraKey;

    private DataSet<PeakDataPlot> CurrentPeaksSet
        => currentSpectraKey is null || peaksStorageProvider.Sets.ContainsKey(currentSpectraKey) is false
            ? peaksStorageProvider.DefaultSet
            : peaksStorageProvider.Sets[currentSpectraKey];

    public IReadOnlyList<PeakDataPlot> CurrentPeaks => CurrentPeaksSet.Data;

    public event Action? OnPlotAreaChanged;

    public Task<IReadOnlyCollection<PeakData>> ExportPeaks(SpectraData spectra)
    {
        var peaks = peaksStorageProvider.Sets.TryGetValue(new SpectraKey(spectra), out var spectraPeaks)
            ? spectraPeaks.Data.Select(p => p.Peak).ToArray()
            : [];

        return Task.FromResult<IReadOnlyCollection<PeakData>>(peaks);
    }

    public Task ImportPeaks(SpectraData spectra, IReadOnlyCollection<PeakData> peaks)
    {
        var spectraKey = new SpectraKey(spectra);

        if (peaksStorageProvider.Sets.TryGetValue(spectraKey, out var spectraPeaks) is false)
        {
            spectraPeaks = new DataSet<PeakDataPlot>(spectraKey.Name, []);
            peaksStorageProvider.AddDataSet(spectraKey, spectraPeaks);
        }

        peaks = peaks.Select(p => p.Copy()).ToArray();

        var plots = peakDataPlotProvider.GetPlots(peaks);

        spectraPeaks.AddRangeThreadSafe(plots);

        if (spectraKey.Equals(currentSpectraKey) is false)
        {
            return Task.CompletedTask;
        }

        peakDataPlotProvider.Draw(plots);
        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task AddPeaksForCurrentSpectra(IReadOnlyCollection<PeakData> peaks)
    {
        var plots = peakDataPlotProvider.GetPlots(peaks);

        peakDataPlotProvider.Draw(plots);

        CurrentPeaksSet.AddRangeThreadSafe(plots);

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task RemovePeaksForCurrentSpectra(IReadOnlyCollection<PeakData> peaks)
    {
        var plots = peakDataPlotProvider.GetPlots(peaks);

        peakDataPlotProvider.Erase(plots);

        CurrentPeaksSet.RemoveRangeThreadSafe(plots);

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    public Task ClearCurrentSpectraPeaks()
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
            if (previousKey is not null && peaksStorageProvider.Sets.ContainsKey(previousKey))
            {
                peakDataPlotProvider.Clear();
                DrawPeaksSet(peaksStorageProvider.DefaultSet);
                OnPlotAreaChanged?.Invoke();
            }

            currentSpectraKey = null;
            return Task.FromResult(false);
        }

        var newKey = new SpectraKey(spectra);

        if (previousKey is not null && newKey.Equals(previousKey))
        {
            return Task.FromResult(peaksStorageProvider.Sets.ContainsKey(newKey));
        }

        var (isCustom, peaksToDraw) = peaksStorageProvider.Sets.TryGetValue(newKey, out var customPeaks)
            ? (true, customPeaks)
            : (false, peaksStorageProvider.DefaultSet);

        peakDataPlotProvider.Clear();
        DrawPeaksSet(peaksToDraw);
        currentSpectraKey = newKey;
        OnPlotAreaChanged?.Invoke();

        return Task.FromResult(isCustom);
    }

    public async Task FitPeaks(IReadOnlyCollection<SpectraData> spectras)
    {
        var settings = new NedlerMeadSettings
        {
            MaxIterationsCount = peakProcessingSettingsMonitor.NedlerMeadMaxIterationsCount,
            RepeatsCount = peakProcessingSettingsMonitor.NedlerMeadRepeatsCount,
            MaxConsecutiveShrinks = peakProcessingSettingsMonitor.NedlerMeadMaxConsecutiveShrinks,
            MinAbsoluteValue = peakProcessingSettingsMonitor.NedlerMeadMinAbsoluteValue,
            MinDeltaPercentageBetweenIterations =
                peakProcessingSettingsMonitor.NedlerMeadMinDeltaPercentageBetweenIterations,
            MaxIterationsWithDeltaLessThanMin =
                peakProcessingSettingsMonitor.NedlerMeadMaxIterationsWithDeltaLessThanMin,
        };

        await Parallel.ForEachAsync(spectras, async (spectra, _) => await FitPeaksInternal(spectra));

        OnPlotAreaChanged?.Invoke();

        return;

        async Task FitPeaksInternal(SpectraData spectra)
        {
            var key = new SpectraKey(spectra);

            if (peaksStorageProvider.Sets.TryGetValue(key, out var customPlotSet))
            {
                if (customPlotSet.Data.IsEmpty())
                {
                    return;
                }

                var peaks = customPlotSet!.Data
                    .Select(d => d.Peak)
                    .ToArray();

                await spectra.FitPeaks(peaks, settings);

                foreach (var plot in customPlotSet.Data)
                {
                    plot.UpdateMarkers();
                }
            }
            else
            {
                var peaks = peaksStorageProvider.DefaultSet.Data
                    .Select(d => d.Peak.Copy())
                    .ToArray();

                if (peaks.IsEmpty())
                {
                    return;
                }

                var plots = peakDataPlotProvider.GetPlots(peaks).ToArray();

                await spectra.FitPeaks(peaks, settings);

                foreach (var peakPlot in plots)
                {
                    peakPlot.UpdateMarkers();
                }

                var plotSet = new DataSet<PeakDataPlot>(key.Name, plots);

                await peaksStorageProvider.AddDataSet(key, plotSet);

                if (key.Equals(currentSpectraKey))
                {
                    peakDataPlotProvider.Clear();
                    DrawPeaksSet(plotSet);
                }
            }
        }
    }

    public async Task<bool> SaveCurrentSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorageProvider.Sets.ContainsKey(currentSpectraKey))
        {
            return false;
        }

        peakDataPlotProvider.Clear();

        var peaksCopy = peaksStorageProvider.DefaultSet.Data
            .Select(d => d.Peak.Copy())
            .ToArray();

        var plotsCopy = peakDataPlotProvider.GetPlots(peaksCopy);

        peakDataPlotProvider.Draw(plotsCopy);

        await peaksStorageProvider.AddDataSet(
            currentSpectraKey,
            new DataSet<PeakDataPlot>(currentSpectraKey.Name, plotsCopy));

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    public async Task<bool> RemoveCurrentSpectraPeaks()
    {
        if (currentSpectraKey is null || peaksStorageProvider.Sets.ContainsKey(currentSpectraKey) is false)
        {
            return false;
        }

        await peaksStorageProvider.DeleteDataSet(currentSpectraKey);

        peakDataPlotProvider.Clear();

        DrawPeaksSet(peaksStorageProvider.DefaultSet);

        OnPlotAreaChanged?.Invoke();

        return true;
    }

    public async Task RemovePeaks(SpectraData data)
    {
        var spectraKey = new SpectraKey(data);

        if (peaksStorageProvider.Sets.ContainsKey(spectraKey) is false)
        {
            return;
        }

        if (spectraKey.Equals(currentSpectraKey))
        {
            await CheckoutSpectra(null);
        }

        await peaksStorageProvider.DeleteDataSet(spectraKey);
    }

    public Task ClearPeaks()
    {
        peakDataPlotProvider.Clear();

        peaksStorageProvider.Clear();

        OnPlotAreaChanged?.Invoke();

        return Task.CompletedTask;
    }

    private void DrawPeaksSet(DataSet<PeakDataPlot> peaks)
    {
        var peakData = peaks.Data
            .Select(x => x.Peak);

        var plots = peakDataPlotProvider.GetPlots(peakData);

        peakDataPlotProvider.Draw(plots);
    }
}
