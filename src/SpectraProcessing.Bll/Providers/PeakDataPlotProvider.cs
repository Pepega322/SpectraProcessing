using System.Collections.Concurrent;
using ScottPlot;
using SpectraProcessing.Bll.Models.ScottPlot.Peak;
using SpectraProcessing.Bll.Providers.Interfaces;
using SpectraProcessing.Domain.Extensions;
using SpectraProcessing.Domain.MathModeling;
using SpectraProcessing.Domain.Models.Peak;

namespace SpectraProcessing.Bll.Providers;

internal sealed class PeakDataPlotProvider : IPeakDataPlotProvider
{
    private static readonly ConcurrentDictionary<PeakData, PeakDataPlot> PeakDataPlots = new();

    private readonly Plot plotForm;

    private readonly IDictionary<PeakData, PeakDataPlot> plotted = new Dictionary<PeakData, PeakDataPlot>();

    public PeakDataPlotProvider(Plot plotForm)
    {
        this.plotForm = plotForm;
        plotForm.Add.Function(x => plotted.Keys.GetPeaksValueAt(x));
    }

    public Task<bool> IsDrew(PeakData data)
    {
        lock (plotted)
        {
            return Task.FromResult(plotted.ContainsKey(data));
        }
    }

    public Task<IReadOnlyList<PeakDataPlot>> GetPlots(IReadOnlyList<PeakData> data)
    {
        var plots = data.Select(GetOrCreatePlot).ToArray();

        return Task.FromResult<IReadOnlyList<PeakDataPlot>>(plots);
    }

    public Task<IReadOnlyList<PeakDataPlot>> Draw(IReadOnlyList<PeakData> data)
    {
        var plots = data
            .Select(d => (Peak: d, Plot: GetOrCreatePlot(d)))
            .ToArray();

        var result = plots.Select(x => x.Plot).ToArray();

        PeakDataPlot[] newPlots;

        lock (plotted)
        {
            newPlots = plots
                .Where(p => plotted.TryAdd(p.Peak, p.Plot))
                .Select(p => p.Plot)
                .ToArray();

            if (newPlots.IsEmpty())
            {
                return Task.FromResult<IReadOnlyList<PeakDataPlot>>(result);
            }
        }

        lock (plotForm)
        {
            foreach (var plot in newPlots)
            {
                foreach (var marker in plot.Markers)
                {
                    plotForm.Add.Plottable(marker);
                }

                plotForm.Add.Plottable(plot.Line);
            }
        }

        return Task.FromResult<IReadOnlyList<PeakDataPlot>>(result);
    }

    public Task<IReadOnlyList<PeakDataPlot>> Erase(IReadOnlyList<PeakData> data)
    {
        var plots = data
            .Select(d => (Peak: d, Plot: GetOrCreatePlot(d)))
            .ToArray();

        var result = plots.Select(x => x.Plot).ToArray();

        PeakDataPlot[] removedPlots;

        lock (plotted)
        {
            removedPlots = plots
                .Where(p => plotted.Remove(p.Peak))
                .Select(p => p.Plot)
                .ToArray();

            if (removedPlots.IsEmpty())
            {
                return Task.FromResult<IReadOnlyList<PeakDataPlot>>(result);
            }
        }

        lock (plotForm)
        {
            foreach (var plot in removedPlots)
            {
                foreach (var marker in plot.Markers)
                {
                    plotForm.Remove(marker);
                }

                plotForm.Remove(plot.Line);
            }
        }

        return Task.FromResult<IReadOnlyList<PeakDataPlot>>(result);
    }

    public Task Clear()
    {
        lock (plotted)
        {
            lock (plotForm)
            {
                foreach (var plot in plotted.Values)
                {
                    foreach (var marker in plot.Markers)
                    {
                        plotForm.Remove(marker);
                    }

                    plotForm.Remove(plot.Line);
                }
            }

            plotted.Clear();
        }

        return Task.CompletedTask;
    }

    private static PeakDataPlot GetOrCreatePlot(PeakData data)
    {
        if (PeakDataPlots.TryGetValue(data, out var plot))
        {
            return plot;
        }

        var newPlot = new PeakDataPlot(data);

        PeakDataPlots.TryAdd(data, newPlot);

        return newPlot;
    }
}
